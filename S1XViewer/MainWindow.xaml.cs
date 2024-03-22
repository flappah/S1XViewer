using Autofac;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Hydrography;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Microsoft.Win32;
using S1XViewer.Base;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Xml;
using static S1XViewer.Model.Interfaces.IDataParser;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for RibbonTestForm.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Autofac.IContainer _container;
        private List<IS1xxDataPackage> _dataPackages = new List<IS1xxDataPackage>();
        private SynchronizationContext? _syncContext;
        private bool _resetViewpoint = true;
        private bool _disposing = false;
        private bool _uiInitializing = true;
        private FeatureToolWindow? _featureToolWindow = null;

        /// <summary>
        ///     Basic initialization
        /// </summary>
        public MainWindow()
        {
            var splashScreen = new SplashScreen(@"images\S1XViewer_SplashScreen.png");
            splashScreen.Show(false, true);

            _container = AutofacInitializer.Initialize();
            var stateStorage = _container.Resolve<IStateStorage>();
            if (stateStorage != null)
            {
                var leftString = stateStorage.Retrieve("WindowPositionLeft");
                if (double.TryParse(leftString, System.Globalization.NumberStyles.Float, new CultureInfo("en-US"), out double left))
                {
                    if (left < System.Windows.SystemParameters.VirtualScreenLeft)
                    {
                        left = System.Windows.SystemParameters.VirtualScreenLeft;
                    }

                    this.Left = left;
                }

                var topString = stateStorage.Retrieve("WindowPositionTop");
                if (double.TryParse(topString, System.Globalization.NumberStyles.Float, new CultureInfo("en-US"), out double top))
                {
                    if (top < System.Windows.SystemParameters.VirtualScreenTop)
                    {
                        top = System.Windows.SystemParameters.VirtualScreenTop;
                    }

                    this.Top = top;
                }

                var widthString = stateStorage.Retrieve("WindowWidth");
                if (double.TryParse(widthString, System.Globalization.NumberStyles.Float, new CultureInfo("en-US"), out double width))
                {
                    this.Width = width;
                }

                var heightString = stateStorage.Retrieve("WindowHeight");
                if (double.TryParse(heightString, System.Globalization.NumberStyles.Float, new CultureInfo("en-US"), out double height))
                {
                    this.Height = height;
                }
            }

            InitializeComponent();

            _syncContext = SynchronizationContext.Current;

            this.Title += " v" + Assembly.GetExecutingAssembly().GetName()?.Version?.ToString() ?? "";

            _ = Task.Factory.StartNew(() =>
            {
                var fileNames = RetrieveRecentFiles();
                int i = 1;
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    foreach (var fileName in fileNames)
                    {
                        var newMenuItem = new RibbonMenuItem
                        {
                            Name = $"MenuItem{i++}",
                            Header = fileName
                        };
                        newMenuItem.Click += AutoOpen_Click;

                        RecentFilesMenuItem.Items.Add(newMenuItem);
                    }

                    var featureRendererManager = _container.Resolve<IFeatureRendererManager>();
                    var colorSchemeNames = featureRendererManager.RetrieveColorRampNames();
                    var optionsStorage = _container.Resolve<IOptionsStorage>();

                    string colorSchemeSelection = optionsStorage.Retrieve("ColorSchemeSelection") ?? string.Empty;
                    colorSchemeSelection = String.IsNullOrEmpty(colorSchemeSelection) ? "default.xml" : colorSchemeSelection;

                    foreach (string colorSchemeName in colorSchemeNames)
                    {
                        var newComboboxItem = new RibbonGalleryItem { Content = colorSchemeName };
                        if (colorSchemeSelection.Equals(colorSchemeName))
                        {
                            newComboboxItem.IsSelected = true;
                        }
                        newComboboxItem.Selected += ColorSchemeItem_Selected;
                        galeryColorSchemes.Items.Add(newComboboxItem);
                    }

                    string basemap = optionsStorage.Retrieve("comboBoxBasemap");
                    BasemapStyle basemapStyle;
                    if (string.IsNullOrEmpty(basemap) == true)
                    {
                        basemapStyle = BasemapStyle.ArcGISTopographic;
                    }
                    else
                    {
                        basemapStyle = Enum.Parse<BasemapStyle>(basemap);
                    }
                    myMapView.Map = new Map(basemapStyle);
                    myMapView.GeoViewTapped += MyMapView_GeoViewTapped;

                    var mapCenterPoint = new MapPoint(0, 50, SpatialReferences.Wgs84);
                    myMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 3000000));

                });
            });

            splashScreen.Close(TimeSpan.FromSeconds(2));
        }

        /// <summary>
        ///     To handle closing the mainform when user presses the window its close button (X)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            var stateStorage = _container.Resolve<IStateStorage>();
            if (stateStorage != null)
            {
                stateStorage.Store("WindowPositionLeft", this.Left.ToString(new CultureInfo("en-US")));
                stateStorage.Store("WindowPositionTop", this.Top.ToString(new CultureInfo("en-US")));
                stateStorage.Store("WindowHeight", this.Height.ToString(new CultureInfo("en-US")));
                stateStorage.Store("WindowWidth", this.Width.ToString(new CultureInfo("en-US")));
            }

            if (_featureToolWindow != null)
            {
                _featureToolWindow.Close();
                _featureToolWindow = null;
            }

            if (_disposing == false)
            {
                var featureRendererFactory = _container.Resolve<IFeatureRendererManager>();
                featureRendererFactory.Clear();

                if (myMapView != null)
                {
                    myMapView.Map = null;
                    myMapView = null;
                }
            }
        }

        #region Ribbon Button and Menu Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public void AppExit_Click(object obj, EventArgs e)
        {
            _disposing = true;
            var featureRendererFactory = _container.Resolve<IFeatureRendererManager>();
            featureRendererFactory.Clear();

            if (myMapView != null)
            {
                myMapView.Map = null;
                myMapView = null; 
            }      

            this.Close();
        }

        /// <summary>
        ///     Handler for recent files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AutoOpen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            var selectedFilename = ((RibbonMenuItem)sender).Tag.ToString() ?? string.Empty;            
            comboboxColorSchemes.IsEnabled = false;

            if (string.IsNullOrEmpty(selectedFilename) == false)
            {
                _resetViewpoint = true;

                if (selectedFilename?.Contains("CATALOG.XML") == false &&
                    selectedFilename?.ToUpper().Contains(".XML") == true || selectedFilename?.ToUpper().Contains(".GML") == true)
                {
                    _ = LoadGMLFile("", selectedFilename);
                }
                else if (selectedFilename?.ToUpper().Contains("CATALOG.XML") == true)
                {
                    _ = LoadExchangeSet(selectedFilename);
                }
                else if (selectedFilename?.ToUpper().Contains(".HDF5") == true || selectedFilename?.ToUpper().Contains(".H5") == true)
                {
                    // filename contains the IHO product standard. First 3 chars indicate the standard to use!
                    string productStandard;
                    if (selectedFilename.Contains(@"\"))
                    {
                        productStandard = selectedFilename.LastPart(@"\").Substring(0, 3);
                    }
                    else
                    {
                        productStandard = selectedFilename.Substring(0, 3);
                    }

                    if (productStandard.IsNumeric() == false)
                    {
                        // if no standard could be determined, ask the user
                        var selectStandardForm = new SelectStandardWindow();
                        selectStandardForm.Owner = Application.Current.MainWindow;
                        selectStandardForm.ShowDialog();
                        productStandard = selectStandardForm.SelectedStandard;
                    }
                    else
                    {
                        productStandard = $"S{productStandard}";
                    }

                    _ = LoadHDF5File(productStandard, selectedFilename, null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ColorSchemeItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            if (_uiInitializing == false)
            {
                if (e.Source is RibbonGalleryItem selectedItem)
                {
                    var optionsStorage = _container.Resolve<IOptionsStorage>();
                    var colorSchemeSelection = String.IsNullOrEmpty(selectedItem.Content?.ToString()) ? "default.xml" : selectedItem.Content?.ToString();
                    optionsStorage.Store("ColorSchemeSelection", colorSchemeSelection);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFileAdd_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "HDF5 files (*.h5;*.hdf5)|*.h5;*.hdf5|ENC files (*.000)|*.031"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                buttonForward.IsEnabled = false;
                buttonForward.Tag = "";
                buttonBackward.IsEnabled = false;
                buttonBackward.Tag = "";
                textBoxTimeValue.Text = string.Empty;
                textBoxTimeValue.Tag = "";
                comboboxColorSchemes.IsEnabled = false;

                var currentFolder = openFileDialog.FileName.Substring(0, openFileDialog.FileName.LastIndexOf(@"\"));
                if (String.IsNullOrEmpty(currentFolder) == false)
                {
                    Directory.SetCurrentDirectory(currentFolder);
                }

                var selectedFilename = openFileDialog.FileName;
                _resetViewpoint = true;

                if (selectedFilename.ToUpper().Contains(".H5") || selectedFilename.ToUpper().Contains(".HDF5"))
                {
                    // filename contains the IHO product standard. First 3 chars indicate the standard to use!
                    string productStandard;
                    if (selectedFilename.Contains(@"\"))
                    {
                        var fileName = selectedFilename.LastPart(@"\");
                        if (fileName.Substring(0, 1) == "S")
                        {
                            productStandard = fileName.Substring(1, 3);
                        }
                        else
                        {
                            productStandard = fileName.Substring(0, 3);
                        }
                    }
                    else
                    {
                        productStandard = selectedFilename.Substring(0, 3);
                    }

                    if (productStandard.IsNumeric() == false)
                    {
                        // if no standard could be determined, ask the user
                        var selectStandardForm = new SelectStandardWindow();
                        selectStandardForm.Owner = Application.Current.MainWindow;
                        selectStandardForm.ShowDialog();
                        productStandard = selectStandardForm.SelectedStandard;
                    }
                    else
                    {
                        productStandard = $"S{productStandard}";
                    }

                    _ = LoadHDF5File(productStandard, selectedFilename, null, false);
                }
                else if (selectedFilename.Contains(".031"))
                {
                    _ = LoadENCFile(selectedFilename);
                }

                buttonRefresh.Tag = selectedFilename;
                buttonRefresh.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void buttonFileOpen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*|XML/GML files (*.xml;*.gml)|*.xml;*.gml|HDF5 files (*.h5;*.hdf5)|*.h5;*.hdf5|ENC files (*.000)|*.031"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                buttonForward.IsEnabled = false;
                buttonForward.Tag = "";
                buttonBackward.IsEnabled = false;
                buttonBackward.Tag = "";
                textBoxTimeValue.Text = string.Empty;
                textBoxTimeValue.Tag = "";
                comboboxColorSchemes.IsEnabled = false;

                var currentFolder = openFileDialog.FileName.Substring(0, openFileDialog.FileName.LastIndexOf(@"\"));
                if (String.IsNullOrEmpty(currentFolder) == false)
                {
                    Directory.SetCurrentDirectory(currentFolder);
                }

                var selectedFilename = openFileDialog.FileName;
                _resetViewpoint = true;

                if (selectedFilename.ToUpper().Contains("CATALOG") && selectedFilename.ToUpper().Contains(".XML"))
                {
                    _ = LoadExchangeSet(selectedFilename);
                }
                else if (selectedFilename.ToUpper().Contains(".XML") || selectedFilename.ToUpper().Contains(".GML"))
                {
                    _ = LoadGMLFile("", selectedFilename);
                }
                else if (selectedFilename.ToUpper().Contains(".H5") || selectedFilename.ToUpper().Contains(".HDF5"))
                {
                    // filename contains the IHO product standard. First 3 chars indicate the standard to use!
                    string productStandard;
                    if (selectedFilename.Contains(@"\"))
                    {
                        var fileName = selectedFilename.LastPart(@"\");
                        if (fileName.Substring(0, 1) == "S")
                        {
                            productStandard = fileName.Substring(1, 3);
                        }
                        else
                        {
                            productStandard = fileName.Substring(0, 3);
                        }
                    }
                    else
                    {
                        productStandard = selectedFilename.Substring(0, 3);
                    }

                    if (productStandard.IsNumeric() == false)
                    {
                        // if no standard could be determined, ask the user
                        var selectStandardForm = new SelectStandardWindow();
                        selectStandardForm.Owner = Application.Current.MainWindow;
                        selectStandardForm.ShowDialog();
                        productStandard = selectStandardForm.SelectedStandard;
                    }
                    else
                    {
                        productStandard = $"S{productStandard}";
                    }

                    _ = LoadHDF5File(productStandard, selectedFilename, null);
                }
                else if (selectedFilename.Contains(".031"))
                {
                    _ = LoadENCFile(selectedFilename);
                }

                buttonRefresh.Tag = selectedFilename;
                buttonRefresh.IsEnabled = true;
            }
        }

        /// <summary>
        /// Clears all layers and resets to initial program open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void buttonClearLayers_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            _dataPackages.Clear();

            Layer? encLayer = myMapView?.Map?.OperationalLayers?.ToList().Find(tp => tp.GetType().ToString().Contains("EncLayer"));

            if (myMapView != null && myMapView.Map != null)
            {
                while (myMapView.Map.OperationalLayers.Count > 0)
                {
                    myMapView.Map.OperationalLayers.RemoveAt(0);
                }

                myMapView.Map.OperationalLayers.Clear();
                if (myMapView.GraphicsOverlays?.Count > 0)
                {
                    myMapView.GraphicsOverlays.RemoveAt(0);
                }
                myMapView.GraphicsOverlays?.Clear();
                myMapView.Map.Tables.Clear();

                if (encLayer != null)
                {
                    myMapView.Map.OperationalLayers?.Add(encLayer);
                }
            }

            var featureRendererFactory = _container.Resolve<IFeatureRendererManager>();
            featureRendererFactory.Clear();            

            var optionsStorage = _container.Resolve<IOptionsStorage>();
            string basemap = optionsStorage.Retrieve("comboBoxBasemap");
            BasemapStyle basemapStyle;
            if (string.IsNullOrEmpty(basemap) == true)
            {
                basemapStyle = BasemapStyle.ArcGISTopographic;
            }
            else
            {
                basemapStyle = Enum.Parse<BasemapStyle>(basemap);
            }
            myMapView.Map = new Map(basemapStyle);

        }

        /// <summary>
        ///     Show options menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void buttonOptions_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            var newOptionsMenu = new OptionsWindow
            {
                Owner = this,
                Container = _container
            };
            newOptionsMenu.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snder"></param>
        /// <param name="e"></param>
        public async void TreeviewItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            var itemDataTable = ((TreeViewItem)sender).Tag as DataTable;
            if (itemDataTable != null)
            {
                dataGridFeatureProperties.ItemsSource = itemDataTable.AsDataView();

                foreach (DataRow row in itemDataTable.Rows)
                {
                    if (row != null && row.ItemArray.Length > 0 && row.ItemArray[0].ToString() == "Id")
                    {
                        foreach (Layer operationalLayer in myMapView.Map.OperationalLayers)
                        {
                            if (operationalLayer is FeatureCollectionLayer featureCollectionLayer)
                            {
                                var queryParameters = new QueryParameters()
                                {
                                    WhereClause = $"FeatureId='{row.ItemArray[1].ToString()}'"
                                };

                                foreach (var table in featureCollectionLayer.FeatureCollection.Tables)
                                {
                                    var featureQueryResult = await table.QueryFeaturesAsync(queryParameters);
                                    if (featureQueryResult.Count() > 0)
                                    {
                                        var feature = featureQueryResult.First();
                                        
                                        featureCollectionLayer?.Layers.ToList().ForEach(l => l.ClearSelection());
                                        
                                        foreach(var layer in featureCollectionLayer?.Layers)
                                        {
                                            if (feature.Geometry is MapPoint && layer.Name.Equals("PointFeatures"))
                                            {
                                                layer.SelectFeature(feature);
                                            }
                                            else if ((feature.Geometry is MapPoint) == false && layer.Name.Equals("PointFeatures") == false)
                                            {
                                                layer.SelectFeature(feature);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     For navigation through HDF5 timeseries. Method relies on the stored values in the Tag properties of the
        ///     backward, forward and textinput controls for proper functioning!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonBackward_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            var firstDateTimeSeries = (DateTime)buttonBackward.Tag;

            var textboxTimeValueTagValues = textBoxTimeValue.Tag?.ToString()?.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            if (textboxTimeValueTagValues?.Length == 3)
            {
                string selectedFileName = textboxTimeValueTagValues[0];
                string productStandard = textboxTimeValueTagValues[1];
                if (DateTime.TryParse(textboxTimeValueTagValues[2], out DateTime selectedDateTime) == true)
                {
                    DateTime proposedDateTime = selectedDateTime.AddHours(-1);
                    _resetViewpoint = false;

                    buttonBackward.IsEnabled = false;
                    buttonForward.IsEnabled = false;

                    textBoxTimeValue.Text = proposedDateTime.ToString("yy-MM-dd HH:mm");
                    textBoxTimeValue.Tag = $"{selectedFileName}#{productStandard}#{proposedDateTime}";
                    await LoadHDF5File(productStandard, selectedFileName, proposedDateTime).ConfigureAwait(true);

                    if (proposedDateTime <= firstDateTimeSeries)
                    {
                        buttonBackward.IsEnabled = false;
                        return;
                    }

                    buttonForward.IsEnabled = true;
                }
            }
        }

        /// <summary>
        ///     For navigation through HDF5 timeseries. Method relies on the stored values in the Tag properties of the
        ///     backward, forward and textinput controls for proper functioning!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            var lastDateTimeSeries = (DateTime)buttonForward.Tag;
            var textboxTimeValueTagValues = textBoxTimeValue.Tag?.ToString()?.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            if (textboxTimeValueTagValues?.Length == 3)
            {
                string selectedFileName = textboxTimeValueTagValues[0];
                string productStandard = textboxTimeValueTagValues[1];
                if (DateTime.TryParse(textboxTimeValueTagValues[2], out DateTime selectedDateTime) == true)
                {
                    DateTime proposedDateTime = selectedDateTime.AddHours(1);
                    _resetViewpoint = false;

                    buttonBackward.IsEnabled = false;
                    buttonForward.IsEnabled = false;

                    textBoxTimeValue.Text = proposedDateTime.ToString("yy-MM-dd HH:mm");
                    textBoxTimeValue.Tag = $"{selectedFileName}#{productStandard}#{proposedDateTime}";
                    await LoadHDF5File(productStandard, selectedFileName, proposedDateTime).ConfigureAwait(true);

                    if (proposedDateTime >= lastDateTimeSeries)
                    {
                        buttonForward.IsEnabled = false;
                        return;
                    }

                    buttonBackward.IsEnabled = true;
                }
            }
        }

        /// <summary>
        ///     To edit and add color schemes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEditColorScheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            try
            {
                var featureRendererManager = _container.Resolve<IFeatureRendererManager>();
                var colorSchemeNames = featureRendererManager.RetrieveColorRampNames();

                var colorSchemesForm = new DefineColourSchemeWindow();
                colorSchemesForm.Owner = Application.Current.MainWindow;
                colorSchemesForm.FeatureRendererManager = featureRendererManager;
                colorSchemesForm.Standard = "S102";
                colorSchemesForm.comboBoxColorSchemes.ItemsSource = colorSchemeNames.ToList();
                colorSchemesForm.ShowDialog();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            var fileName = ((RibbonButton)sender).Tag.ToString();

            if (String.IsNullOrEmpty(fileName) == false)
            {
                if (fileName.ToUpper().Contains(".H5") || fileName.ToUpper().Contains(".HDF5"))
                {
                    // filename contains the IHO product standard. First 3 chars indicate the standard to use!
                    string productStandard;
                    if (fileName.Contains(@"\"))
                    {
                        productStandard = fileName.LastPart(@"\").Substring(0, 3);
                    }
                    else
                    {
                        productStandard = fileName.Substring(0, 3);
                    }

                    if (productStandard.IsNumeric() == false)
                    {
                        // if no standard could be determined, ask the user
                        var selectStandardForm = new SelectStandardWindow();
                        selectStandardForm.Owner = Application.Current.MainWindow;
                        selectStandardForm.ShowDialog();
                        productStandard = selectStandardForm.SelectedStandard;
                    }
                    else
                    {
                        productStandard = $"S{productStandard}";
                    }

                    _ = LoadHDF5File(productStandard, fileName, null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonResetView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            if (myMapView != null && myMapView.Map != null && myMapView.Map.OperationalLayers != null && myMapView.Map.OperationalLayers.Count() > 0)
            {
                List<Envelope> datasetExtents = new List<Envelope>();
                foreach (var layer in myMapView.Map.OperationalLayers)
                {
                    if (layer is FeatureCollectionLayer featureCollectionLayer)
                    {
                        if (featureCollectionLayer.FullExtent != null)
                        {
                            foreach(var table in featureCollectionLayer.FeatureCollection.Tables)
                            {
                                datasetExtents.Add(table.Extent);
                            }
                        }
                    }
                    else if (layer is RasterLayer rasterLayer)
                    {
                        if (rasterLayer.FullExtent != null)
                        {
                            datasetExtents.Add(rasterLayer.FullExtent);
                        }
                    }
                }

                if (datasetExtents.Count > 0)
                {
                    var fullExtent = GeometryEngine.CombineExtents(datasetExtents);
                    await myMapView.SetViewpointAsync(new Viewpoint(fullExtent));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonFindFeature_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            if (textBoxFindValue.Text == null || textBoxFindValue.Text.Length == 0)
            {
                return;
            }

            QueryParameters? queryParameters = null;
            if ((textBoxFindValue.Text.Length == 20 && textBoxFindValue.Text.Substring(0, 3) == "NL_"))
            {
                queryParameters = new QueryParameters()
                {
                    WhereClause = $"FeatureId='{textBoxFindValue.Text}'"
                };
            }
            else
            {
                queryParameters = new QueryParameters()
                {
                    WhereClause = $"UPPER(FeatureName) LIKE '%{textBoxFindValue.Text.ToUpper()}%'"
                };
            }

            var results = new Dictionary<string, DataTable>();
            foreach (Layer operationalLayer in myMapView.Map.OperationalLayers)
            {
                if (operationalLayer is FeatureCollectionLayer featureCollectionLayer)
                {
                    foreach (var table in featureCollectionLayer.FeatureCollection.Tables)
                    {
                        FeatureQueryResult featureQueryResult = await table.QueryFeaturesAsync(queryParameters);
                        if (featureQueryResult.Count() > 0)
                        {
                            Feature idFeature = featureQueryResult.First();
                            featureCollectionLayer?.Layers.ToList().ForEach(l => l.ClearSelection());

                            foreach (var layer in featureCollectionLayer?.Layers)
                            {
                                if (idFeature.Geometry is MapPoint && layer.Name.Equals("PointFeatures"))
                                {
                                    layer.SelectFeature(idFeature);
                                }
                                else if ((idFeature.Geometry is MapPoint) == false && layer.Name.Equals("PointFeatures") == false)
                                {
                                    layer.SelectFeature(idFeature);
                                }
                            }

                            if (idFeature != null && idFeature.Attributes.ContainsKey("FeatureId"))
                            {
                                if (String.IsNullOrEmpty(idFeature.Attributes["FeatureId"]?.ToString()) == false)
                                {
                                    IFeature feature = FindFeature(idFeature.Attributes["FeatureId"].ToString());
                                    if (feature != null)
                                    {
                                        DataTable featureAttributesDataTable = feature.GetData();
                                        string key = (feature is IGeoFeature geoFeature ?
                                            $"{geoFeature.GetType().ToString().LastPart(".")} ({geoFeature.FeatureName.First()?.Name})" :
                                            feature.Id.ToString()) ?? $"No named feature with Id '{feature.Id}'";

                                        if (results.ContainsKey(key))
                                        {
                                            int i = 0;
                                            while (results.ContainsKey($"{key} ({++i})")) ;
                                            key = $"{key} ({i})";
                                        }

                                        results.Add(key, featureAttributesDataTable);
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }

            if (results != null && results.Count > 0)
            {
                dataGridFeatureProperties.ItemsSource = results.First().Value.AsDataView();
                dataGridFeatureProperties.Items.Refresh();

                if (treeViewFeatures.Items.Count > 0)
                {
                    // make a local copy of the treeviewitems
                    TreeViewItem[] tvItemArray = new TreeViewItem[treeViewFeatures.Items.Count];
                    treeViewFeatures.Items.CopyTo(tvItemArray, 0);
                    List<TreeViewItem> tvItemList = tvItemArray.ToList();

                    // remove all non-meta features
                    foreach (TreeViewItem item in treeViewFeatures.Items)
                    {
                        if (item.Tag.Equals("meta") == false)
                        {
                            tvItemList.Remove(item);
                        }
                    }

                    // and restore treeview with meta features
                    treeViewFeatures.Items.Clear();
                    foreach (TreeViewItem item in tvItemList)
                    {
                        treeViewFeatures.Items.Add(item);
                    }
                }

                // add geo features
                var parentTreeNode = new TreeViewItem
                {
                    Header = $"Selected {results.Count.ToString()} geo-feature{(results.Count == 1 ? "" : "s")}",
                    Tag = "feature_count",
                    IsExpanded = true
                };
                treeViewFeatures.Items.Add(parentTreeNode);

                foreach (KeyValuePair<string, DataTable> result in results)
                {
                    TreeViewItem treeNode = new();
                    treeNode.MouseUp += TreeviewItem_Click;
                    treeNode.Header = result.Key;
                    treeNode.Tag = result.Value;

                    parentTreeNode.Items.Add(treeNode);
                }

                    ((TreeViewItem)parentTreeNode.Items[0]).IsSelected = true;
            }
        }

        #endregion

        #region UI logic

        /// <summary>
        ///     Loads the specified exchange set XML file
        /// </summary>
        /// <param name="fullFileName"></param>
        private async Task LoadExchangeSet(string fullFileName)
        {
            var exchangeSetLoader = _container.Resolve<IExchangesetLoader>();
            var xmlDocument = exchangeSetLoader.Load(fullFileName);
            (var originalProductStandard, var productFileNames) = exchangeSetLoader.Parse(xmlDocument);
             
            string selectedFilename = string.Empty;
            var productStandard = originalProductStandard?.Replace("-", "").ToUpper() ?? string.Empty;
            if (productStandard.In("S102", "S104", "S111") == true)
            {
                DateTime? selectedDateTime = DateTime.Now;
                if (productFileNames.Count > 1)
                {
                    var selectDatasetWindow = new SelectDatasetWindow();
                    selectDatasetWindow.Owner = Application.Current.MainWindow;
                    selectDatasetWindow.dataGrid.ItemsSource = exchangeSetLoader.DatasetInfoItems;
                    selectDatasetWindow.ShowDialog();

                    var directory = System.Environment.CurrentDirectory;                   
                    selectedFilename = @$"{directory}\{selectDatasetWindow.SelectedFilename}";
                }
                else if (productFileNames.Count == 1)
                {
                    selectedFilename = productFileNames[0];
                }

                var filename = selectedFilename.LastPart(@"\");
                if (string.IsNullOrEmpty(filename) == false)
                {
                    var xmlNSMgr = new XmlNamespaceManager(xmlDocument.NameTable);
                    xmlNSMgr.AddNamespace("S100XC", "http://www.iho.int/s100/xc/5.0");

                    var producerCodeNode = xmlDocument.DocumentElement?.SelectSingleNode("S100XC:datasetDiscoveryMetadata/S100XC:S100_DatasetDiscoveryMetadata/S100XC:producerCode", xmlNSMgr);
                    var producerCode = string.Empty;

                    if (producerCodeNode == null)
                    {
                        xmlNSMgr = new XmlNamespaceManager(xmlDocument.NameTable);
                        xmlNSMgr.AddNamespace("S100XC", "http://www.iho.int/s100/xc");

                        producerCodeNode = xmlDocument.DocumentElement?.SelectSingleNode("S100XC:datasetDiscoveryMetadata/S100XC:S100_DatasetDiscoveryMetadata/S100XC:producerCode", xmlNSMgr);
                    }

                    if (producerCodeNode != null)
                    {
                        producerCode = producerCodeNode.InnerText.PadRight(4, char.Parse("0"));
                    }

                    var datasetDiscoveryMetadataNode = xmlDocument.DocumentElement?.SelectSingleNode($@"S100XC:datasetDiscoveryMetadata/S100XC:S100_DatasetDiscoveryMetadata[S100XC:fileName='file:/{producerCode}/{filename}']", xmlNSMgr);
                    if (datasetDiscoveryMetadataNode == null)
                    {
                        datasetDiscoveryMetadataNode = xmlDocument.DocumentElement?.SelectSingleNode($@"S100XC:datasetDiscoveryMetadata/S100XC:S100_DatasetDiscoveryMetadata[S100XC:fileName='file:/{originalProductStandard}/DATASET_FILES/{producerCode}/{filename}']", xmlNSMgr);
                    }
                    if (datasetDiscoveryMetadataNode != null && datasetDiscoveryMetadataNode.ChildNodes.Count > 0)
                    {
                        DateTime beginTime = DateTime.Now;
                        var beginTimeNode = datasetDiscoveryMetadataNode.SelectSingleNode(@"S100XC:temporalExtent/S100XC:timeInstantBegin", xmlNSMgr);
                        if (beginTimeNode != null && beginTimeNode.HasChildNodes)
                        {
                            DateTime.TryParse(beginTimeNode.InnerText, out beginTime);
                        }

                        DateTime endTime = DateTime.Now;
                        var endTimeNode = datasetDiscoveryMetadataNode.SelectSingleNode(@"S100XC:temporalExtent/S100XC:timeInstantEnd", xmlNSMgr);
                        if (endTimeNode != null && endTimeNode.HasChildNodes)
                        {
                            DateTime.TryParse(endTimeNode.InnerText, out endTime);
                        }

                        var selectDateTimeWindow = new SelectDateTimeWindow();
                        selectDateTimeWindow.Owner = Application.Current.MainWindow;
                        selectDateTimeWindow.textblockInfo.Text = $"Values available from {beginTime.ToUniversalTime().ToString()} UTC to {endTime.ToUniversalTime().ToString()} UTC. Select a Date and a Time.";
                        selectDateTimeWindow.FirstValidDate = beginTime.ToUniversalTime();
                        selectDateTimeWindow.LastValidDate = endTime.ToUniversalTime();
                        selectDateTimeWindow.datePicker.SelectedDate = beginTime.ToUniversalTime();
                        selectDateTimeWindow.ShowDialog();
                        selectedDateTime = selectDateTimeWindow.SelectedDateTime;

                        buttonBackward.Tag = beginTime.ToUniversalTime();
                        buttonForward.Tag = endTime.ToUniversalTime();
                        textBoxTimeValue.Text = ((DateTime)selectedDateTime).ToString("yy-MM-dd HH:mm");
                        textBoxTimeValue.Tag = $"{selectedFilename}#{productStandard}#{selectedDateTime}";
                    }
                }

                if (String.IsNullOrEmpty(selectedFilename) == false && selectedDateTime != null)
                {
                    await LoadHDF5File(productStandard, selectedFilename, selectedDateTime);
                }
            }
            else if (productFileNames.Count > 1)
            {
                var selectDatasetWindow = new SelectDatasetWindow();
                selectDatasetWindow.Owner = Application.Current.MainWindow; 
                selectDatasetWindow.dataGrid.ItemsSource = exchangeSetLoader.DatasetInfoItems;
                selectDatasetWindow.ShowDialog();

                selectedFilename = selectDatasetWindow.SelectedFilename;
                if (String.IsNullOrEmpty(selectedFilename) == false)
                {
                    await LoadGMLFile(productStandard, selectedFilename);
                }
            }
            else if (productFileNames.Count == 1)
            {
                await LoadGMLFile(productStandard, productFileNames[0]);
            }
        }

        /// <summary>
        ///     Loads the specified ENC file
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// 
        private async Task LoadENCFile(string fileName, bool reinitGeoLayers = true) 
        {
            List<Layer>? nonEncLayers =
                myMapView?.Map?.OperationalLayers.ToList().FindAll(tp => !tp.GetType().ToString().Contains("EncLayer"));

            if (reinitGeoLayers == true)
            {
                myMapView?.Map?.OperationalLayers.Clear();
                myMapView?.GraphicsOverlays?.Clear();
            }

            var myEncExchangeSet = new EncExchangeSet(fileName);
            // Wait for the exchange set to load
            await myEncExchangeSet.LoadAsync();

            // Store a list of data set extent's - will be used to zoom the mapview to the full extent of the Exchange Set
            List<Envelope> dataSetExtents = new List<Envelope>();

            if (myEncExchangeSet.Datasets != null)
            {
                // Add each data set as a layer
                foreach (EncDataset myEncDataset in myEncExchangeSet.Datasets)
                {
                    // Create the cell and layer
                    EncLayer myEncLayer = new EncLayer(new EncCell(myEncDataset));

                    // Add the layer to the map
                    myMapView?.Map?.OperationalLayers.Add(myEncLayer);

                    // Wait for the layer to load
                    await myEncLayer.LoadAsync();

                    if (myEncLayer.FullExtent != null)
                    {
                        // Add the extent to the list of extents
                        dataSetExtents.Add(myEncLayer.FullExtent);
                    }
                }
            }

            if (nonEncLayers != null && nonEncLayers.Any())
            {
                foreach (Layer nonEncLayer in nonEncLayers)
                {
                    myMapView?.Map?.OperationalLayers.Add(nonEncLayer);
                }
            }

            // Use the geometry engine to compute the full extent of the ENC Exchange Set
            Envelope fullExtent = GeometryEngine.CombineExtents(dataSetExtents);

            // Set the viewpoint
            myMapView?.SetViewpoint(new Viewpoint(fullExtent));
        }

        /// <summary>
        ///     Loads the specified HDF5 file
        /// </summary>
        /// <param name="productStandard"></param>
        /// <param name="fileName"></param>
        /// <param name="selectedDateTime"></param>
        /// <param name="reinitGeoLayers"></param>
        private async Task LoadHDF5File(string productStandard, string fileName, DateTime? selectedDateTime, bool reinitGeoLayers = true)
        {
            this.mainRibbon.Title = $"Viewing {fileName.LastPart(@"\")}";
            if (selectedDateTime != null)
            {
                this.mainRibbon.Title += $"@ {((DateTime)selectedDateTime).ToString("yyyy-MM-dd HH:mm")} UTC";
            }

            comboboxColorSchemes.IsEnabled = productStandard == "S102";
            dataGridFeatureProperties.ItemsSource = null;
            treeViewFeatures.Items.Clear();

            if (reinitGeoLayers == true || productStandard != "S102")
            {
                _dataPackages.Clear();

                Layer? encLayer = myMapView?.Map?.OperationalLayers?.ToList().Find(tp => tp.GetType().ToString().Contains("EncLayer"));

                while (myMapView?.Map?.OperationalLayers.Count > 0)
                {
                    myMapView?.Map?.OperationalLayers.RemoveAt(0);
                }

                myMapView?.Map?.OperationalLayers.Clear();
                if (myMapView?.GraphicsOverlays?.Count > 0)
                {
                    myMapView.GraphicsOverlays.RemoveAt(0);
                }
                myMapView?.GraphicsOverlays?.Clear();
                myMapView?.Map?.Tables.Clear();

                if (encLayer != null)
                {
                    myMapView?.Map?.OperationalLayers?.Add(encLayer);
                }
            }

            DateTime? timerStart = DateTime.Now;
            try
            {
                SaveRecentFile(fileName);

                // now find out which data coding format is to be used to determine S1xx data-parser
                IProductSupportFactory productSupportFactory = _container.Resolve<IProductSupportFactory>();
                IProductSupportBase productSupport = productSupportFactory.Create(productStandard);
                short dataCodingFormat = productSupport.GetDataCodingFormat(fileName);

                IDataPackageParser dataParser = _container.Resolve<IDataPackageParser>();
                dataParser.UseStandard = productStandard;
                var dataPackageParser = dataParser.GetDataParser(dataCodingFormat);
                dataPackageParser.Progress += new ProgressFunction((p) =>
                {
                    _syncContext?.Post(new SendOrPostCallback(o =>
                    {
                        if (o != null)
                        {
                            progressBar.Value = (double)o;
                        }
                    }), p); 
                });

                // if there's no selected timeframe, retrieve timeframe from HDF5 file and ask user to select a valid date
                if (selectedDateTime == null && $"{productStandard}/{dataCodingFormat}".In("S111/4") == false)
                {
                    (DateTime start, DateTime end) timeframePresentInFile = await productSupport.RetrieveTimeFrameFromHdfDatasetAsync(fileName);

                    if (timeframePresentInFile.start != DateTime.MinValue &&
                        timeframePresentInFile.end != DateTime.MinValue)
                    {
                        var selectDateTimeWindow = new SelectDateTimeWindow();
                        selectDateTimeWindow.Owner = Application.Current.MainWindow;
                        selectDateTimeWindow.textblockInfo.Text = $"Values available from {timeframePresentInFile.start.ToUniversalTime().ToString()} UTC to {timeframePresentInFile.end.ToUniversalTime().ToString()} UTC. Select a Date and a Time.";
                        selectDateTimeWindow.FirstValidDate = timeframePresentInFile.start.ToUniversalTime();
                        selectDateTimeWindow.LastValidDate = timeframePresentInFile.end.ToUniversalTime();

                        if (timeframePresentInFile.start.ToString("yyyy-MM-dd") == timeframePresentInFile.end.ToString("yyyy-MM-dd"))
                        {
                            // if there's only one date, repopulate the timepicker based on the first and last time present in thhe timeframe. Automatically select the first item
                            selectDateTimeWindow.timePicker.Items.Clear();
                            for (int i = timeframePresentInFile.start.Hour; i <= timeframePresentInFile.end.Hour; i++)
                            {
                                selectDateTimeWindow.timePicker.Items.Add(new ComboBoxItem() { Content = i.ToString("00") + ":00" });
                            }

                            if (selectDateTimeWindow.timePicker.Items != null && selectDateTimeWindow.timePicker.Items.Count > 0)
                            {
                                ((ComboBoxItem)selectDateTimeWindow.timePicker.Items[0]).IsSelected = true;
                            }

                            selectDateTimeWindow.datePicker.IsDropDownOpen = false;
                        }

                        selectDateTimeWindow.datePicker.SelectedDate = timeframePresentInFile.start.ToUniversalTime();
                        selectDateTimeWindow.ShowDialog();
                        selectedDateTime = selectDateTimeWindow.SelectedDateTime;

                        buttonBackward.Tag = timeframePresentInFile.start.ToUniversalTime();
                        buttonForward.Tag = timeframePresentInFile.end.ToUniversalTime();
                        textBoxTimeValue.Text = ((DateTime)selectedDateTime).ToString("yy-MM-dd HH:mm");
                        textBoxTimeValue.Tag = $"{fileName}#{productStandard}#{selectedDateTime}";
                    }
                }

                _syncContext?.Post(new SendOrPostCallback(txt =>
                {
                    labelStatus.Content = $"Loading {txt} ..";

                }), fileName);

                IS1xxDataPackage? dataPackage = await dataPackageParser.ParseAsync(fileName, selectedDateTime).ConfigureAwait(false);
                if (dataPackage != null && dataPackage.Type != S1xxTypes.Null)
                {
                    if (dataPackage.GeoFeatures != null && dataPackage.GeoFeatures.Count() > 0)
                    {
                        // if datapackage contain features display them using FeatureCollection layer(s)
                        _syncContext?.Post(new SendOrPostCallback(async o =>
                        {
                            if (o != null)
                            {
                                CreateFeatureCollection((IS1xxDataPackage)o);
                            }

                            if (string.IsNullOrEmpty(buttonBackward.Tag.ToString()) == false)
                            {
                                if (DateTime.TryParse(buttonBackward.Tag.ToString(), out DateTime beginTime))
                                {
                                    buttonBackward.IsEnabled = selectedDateTime >= beginTime;
                                }
                            }

                            if (string.IsNullOrEmpty(buttonForward.Tag.ToString()) == false)
                            {
                                if (DateTime.TryParse(buttonForward.Tag.ToString(), out DateTime endTime))
                                {
                                    buttonForward.IsEnabled = selectedDateTime < endTime;
                                }
                            }
                        }), dataPackage);

                        _dataPackages.Add(dataPackage);
                    }
                    else if (dataPackage is HdfDataPackage hdfDataPackage)
                    {
                        // contains raster data display them using a RasterLayer
                        _syncContext?.Post(new SendOrPostCallback(async o =>
                        {
                            if (o != null)
                            {
                                CreateRasterCollection((IS1xxDataPackage)o);
                            }
                        }), dataPackage);
                    }
                }
                else if (dataPackage != null && dataPackage.Type == S1xxTypes.Null)
                {
                    MessageBox.Show($"File '{fileName}' currently can't be rendered. No DataParser is able to render the information present in the file!", "Invalid datapackageparser", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"No datapackageparser exists for '{fileName}' and datacodingformat '{dataCodingFormat}'. Can't display data!", "No datapackageparser", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                _syncContext?.Post(new SendOrPostCallback(txt =>
                {
                    labelStatus.Content = "";
                    progressBar.Value = 0;

                }), fileName);

                timerStart = null;
            }
            finally
            {
                string elapsedTime = string.Empty;
                if (timerStart != null)
                {
                    elapsedTime = (DateTime.Now - timerStart).ToString(); 
                }

                _syncContext?.Post(new SendOrPostCallback(o =>
                {
                    if (string.IsNullOrEmpty(elapsedTime) == false)
                    {
                        labelStatus.Content = $"Load time: {o} seconds. Now rendering file ..";
                    }
                }), elapsedTime);
            }
        }

        /// <summary>
        ///     Loads the specified GML file
        /// </summary>
        /// <param name="productStandard">standard to process data on</param>
        /// <param name="fileName">fileName</param>
        private async Task LoadGMLFile(string productStandard, string fileName)
        {
            this.mainRibbon.Title = $"Viewing {fileName.LastPart(@"\")}";

            _dataPackages.Clear();
            dataGridFeatureProperties.ItemsSource = null;
            treeViewFeatures.Items.Clear();

            Layer? encLayer = myMapView?.Map?.OperationalLayers?.ToList().Find(tp => tp.GetType().ToString().Contains("EncLayer"));

            while (myMapView?.Map?.OperationalLayers.Count > 0)
            {
                myMapView.Map.OperationalLayers.RemoveAt(0);
            }

            myMapView?.Map?.OperationalLayers.Clear();
            if (myMapView?.GraphicsOverlays?.Count > 0)
            {
                myMapView.GraphicsOverlays.RemoveAt(0);
            }
            myMapView?.GraphicsOverlays?.Clear();
            myMapView?.Map?.Tables.Clear();

            if (encLayer != null)
            {
                myMapView?.Map?.OperationalLayers?.Add(encLayer);
            }

            var datetimeStart = DateTime.Now;
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);
                SaveRecentFile(fileName);

                _syncContext?.Post(new SendOrPostCallback(txt =>
                {
                    if (txt != null)
                    {
                        labelStatus.Content = $"Loading {txt} ..";
                    }
                }), fileName);

                IDataPackageParser dataParser = _container.Resolve<IDataPackageParser>();
                dataParser.UseStandard = productStandard;
                var dataPackageParser = dataParser.GetDataParser(xmlDoc);
                dataPackageParser.Progress += new ProgressFunction((p) =>
                {
                    _syncContext?.Post(new SendOrPostCallback(o =>
                    {
                        if (o != null)
                        {
                            progressBar.Value = (double)o;
                        }
                    }), p);
                });

                var dataPackage = await dataPackageParser.ParseAsync(xmlDoc).ConfigureAwait(false);
                dataPackage.FileName = fileName;
                if (dataPackage.Type == S1xxTypes.Null)
                {
                    MessageBox.Show($"File '{fileName}' currently can't be rendered. No DataParser is able to render the information present in the file!");
                }
                else
                {
                    _syncContext?.Post(new SendOrPostCallback(o =>
                    {
                        if (o != null)
                        {
                            CreateFeatureCollection((IS1xxDataPackage)o);
                        }
                    }), dataPackage);

                    _syncContext?.Post(new SendOrPostCallback(o =>
                    {
                        if (o != null)
                        {
                            ShowMetaFeatures((IS1xxDataPackage)o);
                        }
                    }), dataPackage);

                    _dataPackages.Add(dataPackage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                var elapsedTime = (DateTime.Now - datetimeStart).ToString();

                _syncContext?.Post(new SendOrPostCallback(txt =>
                {
                    if (txt != null)
                    {
                        labelStatus.Content = $"Load time: {txt} seconds. Now rendering file ..";
                    }
                }), elapsedTime);
                
            }
        }

        /// <summary>
        ///     Extract the meta features from the data package and shows it in the treeview
        /// </summary>
        /// <param name="dataPackage">dataPackage</param>
        private void ShowMetaFeatures(IS1xxDataPackage dataPackage)
        {
            IMetaFeature[] metaFeatures = dataPackage.MetaFeatures;

            if (metaFeatures.Length > 0)
            {
                treeViewFeatures.Items.Clear();

                var parentTreeNode = new TreeViewItem
                {
                    Header = $"Found {metaFeatures.Length} meta-feature{(metaFeatures.Length == 1 ? "" : "s")}",
                    Tag = "meta",
                    IsExpanded = true
                };
                treeViewFeatures.Items.Add(parentTreeNode);

                // insert meta features
                foreach (IMetaFeature metaFeature in metaFeatures)
                {
                    TreeViewItem treeNode = new TreeViewItem();
                    treeNode.MouseUp += TreeviewItem_Click;
                    treeNode.Header = metaFeature?.ToString()?.LastPart(".");
                    treeNode.Tag = metaFeature?.GetData();

                    parentTreeNode.Items.Add(treeNode);
                }
            }
        }

        /// <summary>
        ///     Retrieve recent file from recentfiles.txt
        /// </summary>
        /// <returns>string[]</returns>
        private string[] RetrieveRecentFiles()
        {
            var recentFilesStorage = _container.Resolve<IRecentFilesStorage>();
            var recentFiles = recentFilesStorage.Retrieve("recentfiles");

            if (!String.IsNullOrEmpty(recentFiles))
            {
                return recentFiles.Split(new[] { ',' });
            }

            return new string[0];
        }

        /// <summary>
        ///     Adds a filename to the recent files collection and returns the collection
        /// </summary>
        /// <param name="fileName">fileName</param>
        private string[] AddRecentFile(string fileName)
        {
            string[] recentFiles = RetrieveRecentFiles();

            if (!recentFiles.ToList().Contains(fileName))
            {
                List<string> recentFilesList = recentFiles.ToList();
                recentFilesList.Add(fileName);
                recentFiles = recentFilesList.ToArray();

                var recentFilesStorage = _container.Resolve<IRecentFilesStorage>();
                recentFilesStorage.Store("recentfiles", String.Join(",", recentFiles));
            }

            return recentFiles;
        }

        /// <summary>
        ///     Save entry to the recent files
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveRecentFile(string fileName)
        {
            string[] recentFiles = AddRecentFile(fileName);

            RecentFilesMenuItem.Items.Clear();
            int i = 1;
            foreach (var newFileName in recentFiles)
            {
                var newMenuItem = new RibbonMenuItem
                {
                    Name = $"MenuItem{i++}",
                    Header = newFileName.LastPart(@"\"),
                    CanUserResizeHorizontally = true,
                    Width = 200,
                    ToolTipDescription = newFileName.LastPart(@"\"),
                    ToolTipTitle = $"Recent File",
                    Tag = newFileName
                };
                newMenuItem.Click += AutoOpen_Click;
                RecentFilesMenuItem.Items.Add(newMenuItem);
            }
        }

        /// <summary>
        ///     Finds the specified feature in the S1xx datapackage
        /// </summary>
        /// <param name="featureId">Id value of the feature</param>
        /// <returns>IFeature</returns>
        private IFeature FindFeature(string featureId)
        {
            foreach (IS1xxDataPackage dataPackage in _dataPackages)
            {
                var resultInGeoFeature =
                    dataPackage.GeoFeatures.ToList().Find(ftr => ftr.Id == featureId);

                if (resultInGeoFeature != null)
                {
                    return resultInGeoFeature;
                }

                var resultInMetaFeature =
                    dataPackage.MetaFeatures.ToList().Find(f => f.Id == featureId);

                if (resultInMetaFeature != null)
                {
                    return resultInMetaFeature;
                }

                var resultInInfoFeatures =
                    dataPackage.InformationFeatures.ToList().Find(f => f.Id == featureId);

                if (resultInInfoFeatures != null)
                {
                    return resultInInfoFeatures;
                }
            }

            return null;
        }

        #endregion

        #region ArcGIS Runtime connections

        /// <summary>
        ///     Create a raster and raster layer for rendering on the map
        /// </summary>
        /// <param name="dataPackage"></param>
        private async void CreateRasterCollection(IS1xxDataPackage dataPackage)
        {
            if (myMapView == null || myMapView.Map == null)
            {
                return;
            }

            if (dataPackage is S102DataPackage s102DataPackage)
            {
                var featureRendererManager = _container.Resolve<IFeatureRendererManager>();

                //var clipJsonString = "{\"raster_function\":{\"type\":\"Clip_function\"},\r\n  \"raster_function_arguments\":\r\n  {\r\n    \"minx\":{\"double\":" + s102DataPackage.minX.ToString().Replace(",", ".") + ",\"type\":\"Raster_function_variable\"},\r\n    \"miny\":{\"double\":" + s102DataPackage.minY.ToString().Replace(",", ".") + ",\"type\":\"Raster_function_variable\"},\r\n    \"maxx\":{\"double\":" + s102DataPackage.maxX.ToString().Replace(",", ".") + ",\"type\":\"Raster_function_variable\"},\r\n    \"maxy\":{\"double\":" + s102DataPackage.maxY.ToString().Replace(",", ".") + ",\"type\":\"Raster_function_variable\"},\r\n    \"dx\":{\"double\":" + s102DataPackage.dX.ToString().Replace(",", ".") + ",\"type\":\"Raster_function_variable\"},\r\n    \"dy\":{\"double\":" + s102DataPackage.dY.ToString().Replace(",", ".") + ",\"type\":\"Raster_function_variable\"},\r\n    \"raster\":{\"name\":\"raster\",\"is_raster\":true,\"type\":\"Raster_function_variable\"},\r\n    \"type\":\"Raster_function_arguments\"\r\n  },\r\n  \"type\":\"Raster_function_template\"\r\n}";
                var nodataJsonString = "{\"raster_function\":{\"type\":\"Mask_function\"},\r\n  \"raster_function_arguments\":\r\n  {\r\n    \"nodata_values\":{\"double_array\":[" + s102DataPackage.noDataValue.ToString().Replace(",", ".") + "],\"type\":\"Raster_function_variable\"},\r\n    \"nodata_interpretation\":{\"nodata_interpretation\":\"all\",\"type\":\"Raster_function_variable\"},\r\n    \"raster\":{\"name\":\"raster\",\"is_raster\":true,\"type\":\"Raster_function_variable\"},\r\n    \"type\":\"Raster_function_arguments\"\r\n  },\r\n  \"type\":\"Raster_function_template\"\r\n}";

                var nodataRasterFunction = RasterFunction.FromJson(nodataJsonString);
                if (nodataRasterFunction == null)
                {
                    return;
                }

                var arguments = nodataRasterFunction?.Arguments;
                if (arguments == null)
                {
                    return;
                }

                var rasterNames = arguments?.GetRasterNames();
                if (rasterNames == null || rasterNames.Count == 0)
                {
                    return;
                }

                try
                {
                    var raster = new Raster(s102DataPackage.TiffFileName);
                    arguments?.SetRaster(rasterNames[0], raster);

                    var nodataRaster = new Raster(nodataRasterFunction);
                    var rasterLayer = new RasterLayer(nodataRaster);

                    Colormap? colormap = featureRendererManager.GetColormap(comboboxColorSchemes.Text, dataPackage.Type.ToString());
                    rasterLayer.Renderer = new ColormapRenderer(colormap);

                    rasterLayer.Loaded += (s, e) => Dispatcher.Invoke(async () =>
                    {
                        try
                        {
                            if (_resetViewpoint == true && rasterLayer.FullExtent != null)
                            {
                                await myMapView.SetViewpointAsync(new Viewpoint(rasterLayer.FullExtent));
                                _resetViewpoint = false;
                            }

                            _syncContext?.Post(new SendOrPostCallback(o =>
                            {
                                labelStatus.Content = labelStatus.Content.ToString()?.Replace(" Now rendering file ..", "");
                                progressBar.Value = 0;
                            }), null);

                            BackgroundWorker bgw = new();
                            bgw.DoWork += delegate
                            {
                                Task.Delay(5000).Wait();
                            };
                            bgw.RunWorkerCompleted += delegate
                            {
                                _syncContext?.Post(new SendOrPostCallback((o) =>
                                {
                                    labelStatus.Content = "";
                                }), "");
                            };
                            bgw.RunWorkerAsync();
                        }
                        catch (Exception) { }
                    });

                    myMapView?.Map?.OperationalLayers.Add(rasterLayer);
                    await rasterLayer.LoadAsync().ConfigureAwait(true);

                    if (rasterLayer.FullExtent != null)
                    {
                        await myMapView.SetViewpointGeometryAsync(rasterLayer.FullExtent, 15).ConfigureAwait(false);
                    }

                    nodataRaster = null;
                    raster = null;
                }
                catch(Exception ex)
                {
                    _syncContext?.Post(new SendOrPostCallback((o) =>
                    {
                        labelStatus.Content = "";
                        progressBar.Value = 0;

                    }), "");

                    MessageBox.Show($"Can't read file {s102DataPackage.TiffFileName}.\n{ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        ///     Creation a feature collection for rendering on the map
        /// </summary>
        /// <param name="dataPackage">S1xx dataPackage</param>
        private async void CreateFeatureCollection(IS1xxDataPackage dataPackage)
        {
            Field idField = new Field(FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(FieldType.Text, "FeatureName", "Name", 255);

            List<Field> polyFields = new List<Field>
            {
                idField,
                nameField
            };

            List<Field> pointFields = new List<Field>
            {
                idField,
                nameField
            };

            List<Field> pointVectorFields = new List<Field>
            {
                idField,
                nameField,
            };

            List<Field> lineFields = new List<Field>
            {
                idField,
                nameField
            };

            if (dataPackage.GeoFeatures == null || dataPackage.GeoFeatures.Count() == 0)
            {
                return;
            }

            if (_resetViewpoint == true)
            {
                Envelope fullExtent = (Envelope)dataPackage.BoundingBox;
                if (fullExtent != null)
                {
                    await myMapView.SetViewpointAsync(new Viewpoint(fullExtent));
                }
            }

            // set projection to the srs of the first valid geometry
            int i = 0;
            while (dataPackage.GeoFeatures[i++].Geometry == null && i < dataPackage.GeoFeatures.Length) ;
            SpatialReference? horizontalCRS = SpatialReferences.Wgs84;
            if (i - 1 < dataPackage.GeoFeatures.Length)
            {
                horizontalCRS = dataPackage.GeoFeatures[i].Geometry.SpatialReference;
            }

            var featureRendererManager = _container.Resolve<IFeatureRendererManager>();
            featureRendererManager.Clear();

            var colorSchemeName = String.IsNullOrEmpty(comboboxColorSchemes.Text) ? "default.xml" : comboboxColorSchemes.Text;
            featureRendererManager.LoadColorRamp(colorSchemeName, dataPackage.Type.ToString().LastPart("."));

            var polygonsTable = featureRendererManager.Create("PolygonFeatures", polyFields, GeometryType.Polygon, horizontalCRS);
            var linesTable = featureRendererManager.Create("LineFeatures", lineFields, GeometryType.Polyline, horizontalCRS);
            var pointsTable = featureRendererManager.Create("PointFeatures", pointFields, GeometryType.Point, horizontalCRS);
            var vectorsTable = featureRendererManager.Create("VectorFeatures", pointVectorFields, GeometryType.Point, horizontalCRS, true);

            var graphicList = new List<Graphic>();
            var pointFeatureList = new List<Feature>();
            var lineFeatureList = new List<Feature>();
            var polygonFeatureList = new List<Feature>();
            var vectorFeatureList = new List<Feature>();
            var filledPolyFeatureLists = new Dictionary<string, List<Feature>>();

            //Parallel.ForEach(dataPackage.GeoFeatures, feature =>
            foreach (var feature in dataPackage.GeoFeatures)
            {
                if (feature is IGeoFeature geoFeature)
                {
                    (string type, Feature feature, Graphic? graphic) renderedFeature = geoFeature.Render(featureRendererManager, horizontalCRS);

                    switch (renderedFeature.type)
                    {
                        case "PointFeatures":
                            pointFeatureList.Add(renderedFeature.feature);
                            break;

                        case "LineFeatures":
                            lineFeatureList.Add(renderedFeature.feature);
                            break;

                        case "PolygonFeatures":
                            polygonFeatureList.Add(renderedFeature.feature);
                            break;

                        case "VectorFeatures":
                            vectorFeatureList.Add(renderedFeature.feature);
                            break;

                        default:
                            if (filledPolyFeatureLists.ContainsKey(renderedFeature.type) == false)
                            {
                                filledPolyFeatureLists.Add(renderedFeature.type, new List<Feature>());
                            }
                            filledPolyFeatureLists[renderedFeature.type].Add(renderedFeature.feature);

                            break;
                    }

                    if (renderedFeature.graphic != null)
                    {
                        graphicList.Add(renderedFeature.graphic);
                    }
                }
            }

            await pointsTable.AddFeaturesAsync(pointFeatureList);
            await linesTable.AddFeaturesAsync(lineFeatureList);
            await polygonsTable.AddFeaturesAsync(polygonFeatureList);
            await vectorsTable.AddFeaturesAsync(vectorFeatureList);

            var filledPolygonTables = new List<FeatureCollectionTable>();
            foreach (KeyValuePair<string, List<Feature>> filledPolyFeatureList in filledPolyFeatureLists)
            {
                FeatureCollectionTable? filledPolygonTable = featureRendererManager.Get(filledPolyFeatureList.Key);
                if (filledPolygonTable != null)
                {
                    filledPolygonTables.Add(filledPolygonTable);
                    filledPolygonTable.AddFeaturesAsync(filledPolyFeatureList.Value); // no await applied since this speeds up rendering in the UI! 
                }
            }

            var featuresCollection = new FeatureCollection();

            if (filledPolygonTables.Count > 0)
            {
                foreach (FeatureCollectionTable filledPolysTable in filledPolygonTables)
                {
                    featuresCollection.Tables.Add(filledPolysTable);
                }
            }

            if (pointsTable.Count() > 0)
            {
                featuresCollection.Tables.Add(pointsTable);
            }

            if (vectorsTable.Count() > 0)
            {
                featuresCollection.Tables.Add(vectorsTable);
            }

            if (polygonsTable.Count() > 0)
            {
                featuresCollection.Tables.Add(polygonsTable);
            }

            if (linesTable.Count() > 0)
            {
                featuresCollection.Tables.Add(linesTable);
            }

            if (myMapView != null && myMapView.Map != null)
            {
                try
                {
                    var graphicsOverlay = new GraphicsOverlay() { Id = "VectorFeatures" };
                    graphicsOverlay.Graphics.AddRange(graphicList);
                    myMapView.GraphicsOverlays?.Add(graphicsOverlay);
                }
                catch (Exception) { }

                // Create a label definition from the JSON string. 
                var idLabelDefinition = LabelDefinition.FromJson(@"{
                        ""labelExpressionInfo"":{""expression"":""return $feature.FeatureName""},
                        ""labelPlacement"":""esriServerPolygonPlacementAlwaysHorizontal"",
                        ""symbol"":
                            { 
                                ""angle"":0,
                                ""backgroundColor"":[0,0,0,0],
                                ""borderLineColor"":[0,0,0,0],
                                ""borderLineSize"":0,
                                ""color"":[0,0,255,255],
                                ""font"":
                                    {
                                        ""decoration"":""none"",
                                        ""size"":8,
                                        ""style"":""normal"",
                                        ""weight"":""normal""
                                    },
                                ""haloColor"":[255,255,255,255],
                                ""haloSize"":0.1,
                                ""horizontalAlignment"":""center"",
                                ""kerning"":false,
                                ""type"":""esriTS"",
                                ""verticalAlignment"":""middle"",
                                ""xoffset"":0,
                                ""yoffset"":0
                            }
                    }");

                var collectionLayer = new FeatureCollectionLayer(featuresCollection);
                // Add the layer to the Map's Operational Layers collection
                myMapView.Map.OperationalLayers.Add(collectionLayer);
                // When the layer loads, zoom the map view to the extent of the feature collection
                collectionLayer.Loaded += (s, e) => Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        List<Envelope> datasetExtents = new List<Envelope>();
                        foreach (FeatureLayer layer in collectionLayer.Layers)
                        {
                            if (idLabelDefinition != null)
                            {
                                layer.LabelDefinitions.Add(idLabelDefinition);
                                layer.LabelsEnabled = true;
                            }

                            if (layer.FullExtent != null && layer.FullExtent.MMin != double.NaN && layer.FullExtent.MMax != double.NaN)
                            {
                                datasetExtents.Add(layer.FullExtent);
                            }
                        }

                        if (_resetViewpoint == true && datasetExtents.Count > 0)
                        {
                            var fullExtent = GeometryEngine.CombineExtents(datasetExtents);
                            await myMapView.SetViewpointAsync(new Viewpoint(fullExtent));
                            _resetViewpoint = false;
                        }

                        _syncContext?.Post(new SendOrPostCallback(o =>
                        {
                            labelStatus.Content = labelStatus.Content.ToString()?.Replace(" Now rendering file ..", "");
                            progressBar.Value = 0;

                            textBoxFindValue.IsEnabled = true;
                            buttonFindFeature.IsEnabled = true;
                        }), null);

                        BackgroundWorker bgw = new();
                        bgw.DoWork += delegate
                        {
                            Task.Delay(5000).Wait();
                        };
                        bgw.RunWorkerCompleted += delegate
                        {
                            _syncContext?.Post(new SendOrPostCallback((o) =>
                            {
                                labelStatus.Content = "";
                            }), "");
                        };
                        bgw.RunWorkerAsync();
                    }
                    catch (Exception) { }
                });
            }
        }

        /// <summary>
        ///     If the map view is tapped handle results from the selected feature / object
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private async void MyMapView_GeoViewTapped(object? sender, GeoViewInputEventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            if (e is null)
            {
                return;
            }

            if (myMapView == null || myMapView.Map == null || myMapView.Map.OperationalLayers == null)
            {
                return;
            }

            try
            {
                // get the tap location in screen units
                System.Windows.Point tapScreenPoint = e.Position;

                // Specify identify properties.
                double pixelTolerance = 1.0;
                bool returnPopupsOnly = false;
                int maxResults = 5;

                // Identify a  group layer using MapView, passing in the layer, the tap point, tolerance, types to return, and max results.

                var results = new Dictionary<string, DataTable>();
                foreach (Layer operationalLayer in myMapView.Map.OperationalLayers)
                {
                    if (operationalLayer is FeatureCollectionLayer featureCollectionLayer)
                    {
                        IdentifyLayerResult groupLayerResult =
                            await myMapView.IdentifyLayerAsync(featureCollectionLayer, tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);

                        if (groupLayerResult.SublayerResults.Count > 0)
                        {
                            // Iterate each set of child layer results.
                            foreach (IdentifyLayerResult subLayerResult in groupLayerResult.SublayerResults)
                            {
                                // clear featureselection in all layers
                                featureCollectionLayer?.Layers.ToList().ForEach(l => l.ClearSelection());

                                // Iterate each geoelement in the child layer result set.
                                foreach (GeoElement idElement in subLayerResult.GeoElements)
                                {
                                    // cast the result GeoElement to Feature
                                    Feature idFeature = idElement as Feature;

                                    // select this feature in the feature layer
                                    var layer = subLayerResult.LayerContent as FeatureLayer;
                                    if (layer != null && idFeature != null)
                                    {
                                        layer.SelectFeature(idFeature);
                                    }

                                    if (idElement != null && idElement.Attributes.ContainsKey("FeatureId"))
                                    {
                                        if (String.IsNullOrEmpty(idElement.Attributes["FeatureId"]?.ToString()) == false)
                                        {
                                            IFeature feature = FindFeature(idElement.Attributes["FeatureId"].ToString());
                                            if (feature != null)
                                            {
                                                DataTable featureAttributesDataTable = feature.GetData();
                                                string key = (feature is IGeoFeature geoFeature ?
                                                    $"{geoFeature.GetType().ToString().LastPart(".")} ({geoFeature.FeatureName.First()?.Name})" :
                                                    feature.Id.ToString()) ?? $"No named feature with Id '{feature.Id}'";

                                                if (results.ContainsKey(key))
                                                {
                                                    int i = 0;
                                                    while (results.ContainsKey($"{key} ({++i})")) ;
                                                    key = $"{key} ({i})";
                                                }

                                                results.Add(key, featureAttributesDataTable);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                
                    if (operationalLayer is RasterLayer rasterLayer)
                    {
                         IdentifyLayerResult groupLayerResult =
                            await myMapView.IdentifyLayerAsync(rasterLayer, tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);

                        if (groupLayerResult.GeoElements.Any())
                        {
                            var dataTable = new DataTable($"Results_{this.GetHashCode()}");
                            dataTable.Columns.AddRange(new DataColumn[]
                            {
                                new DataColumn
                                {
                                    DataType = System.Type.GetType("System.String"),
                                    ColumnName = "Name"
                                },
                                 new DataColumn
                                {
                                    DataType = System.Type.GetType("System.String"),
                                    ColumnName = "Value"
                                }
                            });

                            int keyNumber = 0;
                            foreach (GeoElement geoElement in groupLayerResult.GeoElements)
                            {
                                DataRow row = dataTable.NewRow();
                                row["Name"] = "Geometry";
                                row["Value"] = geoElement.Geometry;

                                foreach (var attribute in geoElement.Attributes)
                                {
                                    row = dataTable.NewRow();
                                    row["Name"] = attribute.Key;
                                    row["Value"] = attribute.Value;
                                    dataTable.Rows.Add(row);
                                }

                                string key;
                                if (geoElement.Geometry != null)
                                {
                                    Geometry wgs84Geometry = GeometryEngine.Project(geoElement.Geometry, SpatialReference.Create(4326));
                                    key = wgs84Geometry.ToString() ?? $"point_{++keyNumber}";
                                }
                                else
                                {
                                    key = $"point_{++keyNumber}";
                                }

                                if (results.ContainsKey(key))
                                {
                                    int i = 0;
                                    while (results.ContainsKey($"{key} ({++i})")) ;
                                    key = $"{key} ({i})";
                                }

                                results.Add(key, dataTable);
                            }
                        }
                    }
                }

                if (_featureToolWindow != null)
                {
                    _featureToolWindow.Close();
                    _featureToolWindow = null;
                }

                if (results != null && results.Count > 0)
                {
                    if (results.First().Value.Rows.Count > 0)
                    {
                        var containsFeatureToolWindow = results.First().Value.Rows.ContainsKey("FeatureToolWindow");

                        if (containsFeatureToolWindow.result == true && containsFeatureToolWindow.value.ToString().Equals("True"))
                        {
                            _featureToolWindow = new FeatureToolWindow();
                            _featureToolWindow.TemplateFields = results.First().Value.Rows.ContainsKey("FeatureToolWindowTemplate").value.ToString() ?? "";
                            _featureToolWindow.FieldCollection = results.First().Value.Rows;
                            _featureToolWindow.Left = tapScreenPoint.X + this.Left - 50.0;
                            _featureToolWindow.Top = tapScreenPoint.Y + this.Top + 100.0;
                            _featureToolWindow.Show();
                        }
                    }

                    dataGridFeatureProperties.ItemsSource = results.First().Value.AsDataView();
                    dataGridFeatureProperties.Items.Refresh();

                    if (treeViewFeatures.Items.Count > 0)
                    {
                        // make a local copy of the treeviewitems
                        TreeViewItem[] tvItemArray = new TreeViewItem[treeViewFeatures.Items.Count];
                        treeViewFeatures.Items.CopyTo(tvItemArray, 0);
                        List<TreeViewItem> tvItemList = tvItemArray.ToList();

                        // remove all non-meta features
                        foreach (TreeViewItem item in treeViewFeatures.Items)
                        {
                            if (item.Tag.Equals("meta") == false)
                            {
                                tvItemList.Remove(item);
                            }
                        }

                        // and restore treeview with meta features
                        treeViewFeatures.Items.Clear();
                        foreach (TreeViewItem item in tvItemList)
                        {
                            treeViewFeatures.Items.Add(item);
                        }
                    }

                    // add geo features
                    var parentTreeNode = new TreeViewItem
                    {
                        Header = $"Selected {results.Count.ToString()} geo-feature{(results.Count == 1 ? "" : "s")}",
                        Tag = "feature_count",
                        IsExpanded = true
                    };
                    treeViewFeatures.Items.Add(parentTreeNode);

                    foreach (KeyValuePair<string, DataTable> result in results)
                    {
                        TreeViewItem treeNode = new();
                        treeNode.MouseUp += TreeviewItem_Click;
                        treeNode.Header = result.Key;
                        treeNode.Tag = result.Value;

                        parentTreeNode.Items.Add(treeNode);
                    }

                    ((TreeViewItem)parentTreeNode.Items[0]).IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Feature selection error ({ex.Message})", ex.ToString());
            }
        }

        #endregion

    }
}
