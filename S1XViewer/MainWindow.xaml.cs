using Autofac;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Hydrography;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
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
        private string _selectedFilename = string.Empty;
        private bool _resetViewpoint = true;

        /// <summary>
        ///     Basic initialization
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _container = AutofacInitializer.Initialize();
            _syncContext = SynchronizationContext.Current;

            this.Title += " v" + Assembly.GetExecutingAssembly().GetName()?.Version?.ToString() ?? "";

            _ = Task.Factory.StartNew(() =>
            {
                var fileNames = RetrieveRecentFiles();
                int i = 1;
                foreach (var fileName in fileNames)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        var newMenuItem = new RibbonMenuItem
                        {
                            Name = $"MenuItem{i++}",
                            Header = fileName
                        };
                        newMenuItem.Click += AutoOpen_Click;

                        RecentFilesMenuItem.Items.Add(newMenuItem);
                    });
                }
            });

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

            var mapCenterPoint = new MapPoint(0, 50, SpatialReferences.Wgs84);
            myMapView.SetViewpoint(new Viewpoint(mapCenterPoint, 3000000));
        }

        #region Menu Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public void AppExit_Click(object obj, EventArgs e)
        {
            var featureRendererFactory = _container.Resolve<IFeatureCollectionFactory>();
            featureRendererFactory.Clear();

            if (myMapView != null)
            {
                myMapView.Map = null;
                myMapView = null; 
            }      

            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AppOpen_Click(object sender, RoutedEventArgs e)
        {
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

                var currentFolder = openFileDialog.FileName.Substring(0, openFileDialog.FileName.LastIndexOf(@"\"));
                if (String.IsNullOrEmpty(currentFolder) == false)
                {
                    Directory.SetCurrentDirectory(currentFolder);
                }

                _selectedFilename = openFileDialog.FileName;
                _resetViewpoint = true;

                if (_selectedFilename.ToUpper().Contains("CATALOG") && _selectedFilename.ToUpper().Contains(".XML"))
                {
                    _ = LoadExchangeSet(_selectedFilename);
                }
                else if (_selectedFilename.ToUpper().Contains(".XML") || _selectedFilename.ToUpper().Contains(".GML"))
                {
                    _ = LoadGMLFile("", _selectedFilename);
                }
                else if (_selectedFilename.ToUpper().Contains(".H5") || _selectedFilename.ToUpper().Contains(".HDF5"))
                {
                    // filename contains the IHO product standard. First 3 chars indicate the standard to use!
                    string productStandard;
                    if (_selectedFilename.Contains(@"\"))
                    {
                        productStandard = _selectedFilename.LastPart(@"\").Substring(0, 3);
                    }
                    else
                    {
                        productStandard = _selectedFilename.Substring(0, 3);
                    }

                    if (productStandard.IsNumeric() == false)
                    {


                        // if no standard could be determined, ask the user
                        var selectStandardForm = new SelectStandardWindow();
                        selectStandardForm.Owner = this;
                        selectStandardForm.ShowDialog();
                        productStandard = selectStandardForm.SelectedStandard;
                    }
                    else
                    {
                        productStandard = $"S{productStandard}";
                    }

                    _ = LoadHDF5File(productStandard, _selectedFilename, null);
                }
                else if (_selectedFilename.Contains(".031"))
                {
                    _ = LoadENCFile(_selectedFilename);
                }
            }
        }

        /// <summary>
        /// Clears all layers and resets to initial program open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ResetLayers_Click(object sender, RoutedEventArgs e)
        {
            _dataPackages.Clear();
            dataGridFeatureProperties.ItemsSource = null;
            treeViewFeatures.Items.Clear();
            myMapView?.Map?.OperationalLayers.Clear();
        }

        /// <summary>
        ///     Show options menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OptionsMenu_Click(object sender, RoutedEventArgs e)
        {
            var newOptionsMenu = new OptionsWindow
            {
                Container = _container
            };
            newOptionsMenu.Show();
        }

        /// <summary>
        ///     Handler for recent files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AutoOpen_Click(object sender, RoutedEventArgs e)
        {
            _selectedFilename = ((MenuItem)sender).Tag.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(_selectedFilename) == false)
            {
                if (_selectedFilename?.Contains("CATALOG.XML") == false &&
                    _selectedFilename?.ToUpper().Contains(".XML") == true || _selectedFilename?.ToUpper().Contains(".GML") == true)
                {
                    _= LoadGMLFile("", _selectedFilename);
                }
                else if (_selectedFilename?.ToUpper().Contains("CATALOG.XML") == true)
                {
                    _ = LoadExchangeSet(_selectedFilename);
                }
                else if (_selectedFilename?.ToUpper().Contains(".HDF5") == true || _selectedFilename?.ToUpper().Contains(".H5") == true)
                {
                    // filename contains the IHO product standard. First 3 chars indicate the standard to use!
                    string productStandard;
                    if (_selectedFilename.Contains(@"\"))
                    {
                        productStandard = _selectedFilename.LastPart(@"\").Substring(0, 3);
                    }
                    else
                    {
                        productStandard = _selectedFilename.Substring(0, 3);
                    }

                    if (productStandard.IsNumeric() == false)
                    {
                        // if no standard could be determined, ask the user
                        var selectStandardForm = new SelectStandardWindow();
                        selectStandardForm.Owner = this;
                        selectStandardForm.ShowDialog();
                        productStandard = selectStandardForm.SelectedStandard;
                    }
                    else
                    {
                        productStandard = $"S{productStandard}";
                    }

                    _ = LoadHDF5File(productStandard, _selectedFilename, null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snder"></param>
        /// <param name="e"></param>
        public void TreeviewItem_Click(object sender, RoutedEventArgs e)
        {
            var itemDataTable = ((TreeViewItem)sender).Tag as DataTable;
            if (itemDataTable != null)
            {
                dataGridFeatureProperties.ItemsSource = itemDataTable.AsDataView();
            }
        }

        /// <summary>
        ///     For navigation through HDF5 timeseries. Method relies on the stored values in the Tag properties of the
        ///     backward, forward and textinput controls for proper functioning!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBackward_Click(object sender, RoutedEventArgs e)
        {
            var firstDateTimeSeries = (DateTime)buttonBackward.Tag;

            var textboxTimeValueTagValues = textBoxTimeValue.Tag?.ToString()?.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (textboxTimeValueTagValues?.Length == 2)
            {
                string productStandard = textboxTimeValueTagValues[0];
                if (DateTime.TryParse(textboxTimeValueTagValues[1], out DateTime selectedDateTime) == true)
                {
                    DateTime proposedDateTime = selectedDateTime.AddHours(-1);
                    if (proposedDateTime <= firstDateTimeSeries)
                    {
                        buttonBackward.IsEnabled = false;
                        return;
                    }
                    buttonForward.IsEnabled = true;
                    _resetViewpoint = false;

                    textBoxTimeValue.Text = proposedDateTime.ToString("yy-MM-dd HH:mm");
                    textBoxTimeValue.Tag = $"{productStandard}_{proposedDateTime}";
                    _ = LoadHDF5File(productStandard, _selectedFilename, proposedDateTime);
                }
            }
        }

        /// <summary>
        ///     For navigation through HDF5 timeseries. Method relies on the stored values in the Tag properties of the
        ///     backward, forward and textinput controls for proper functioning!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            var lastDateTimeSeries = (DateTime)buttonForward.Tag;
            var textboxTimeValueTagValues = textBoxTimeValue.Tag?.ToString()?.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (textboxTimeValueTagValues?.Length == 2)
            {
                string productStandard = textboxTimeValueTagValues[0];
                if (DateTime.TryParse(textboxTimeValueTagValues[1], out DateTime selectedDateTime) == true)
                {
                    DateTime proposedDateTime = selectedDateTime.AddHours(1);
                    if (proposedDateTime >= lastDateTimeSeries)
                    {
                        buttonBackward.IsEnabled = false;
                        return;
                    }
                    buttonBackward.IsEnabled = true;
                    _resetViewpoint = false;

                    textBoxTimeValue.Text = proposedDateTime.ToString("yy-MM-dd HH:mm");
                    textBoxTimeValue.Tag = $"{productStandard}_{proposedDateTime}";
                    _ = LoadHDF5File(productStandard, _selectedFilename, proposedDateTime);
                }
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
            (var productStandard, var productFileNames) = exchangeSetLoader.Parse(xmlDocument);

            productStandard = productStandard?.Replace("-", "").ToUpper() ?? string.Empty;
            if (productStandard.In("S104", "S111") == true)
            {
                DateTime? selectedDateTime = DateTime.Now;
                if (productFileNames.Count > 1)
                {
                    var selectDatasetWindow = new SelectDatasetWindow();
                    selectDatasetWindow.Owner = this;
                    selectDatasetWindow.dataGrid.ItemsSource = exchangeSetLoader.DatasetInfoItems;
                    selectDatasetWindow.ShowDialog();
                    _selectedFilename = selectDatasetWindow.SelectedFilename;

                    var filename = _selectedFilename.LastPart(@"\");
                    if (string.IsNullOrEmpty(filename) == false)
                    {
                        var xmlNSMgr = new XmlNamespaceManager(xmlDocument.NameTable);
                        xmlNSMgr.AddNamespace("S100XC", "http://www.iho.int/s100/xc");

                        var datasetDiscoveryMetadataNode = xmlDocument.DocumentElement?.SelectSingleNode($@"S100XC:datasetDiscoveryMetadata/S100XC:S100_DatasetDiscoveryMetadata[S100XC:fileName='{filename}']", xmlNSMgr);
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
                            selectDateTimeWindow.Owner = this;
                            selectDateTimeWindow.textblockInfo.Text = $"Values available from {beginTime.ToUniversalTime().ToString()} UTC to {endTime.ToUniversalTime().ToString()} UTC. Select a Date and a Time.";
                            selectDateTimeWindow.FirstValidDate = beginTime.ToUniversalTime();
                            selectDateTimeWindow.LastValidDate = endTime.ToUniversalTime();
                            selectDateTimeWindow.datePicker.SelectedDate = beginTime.ToUniversalTime();
                            selectDateTimeWindow.ShowDialog();
                            selectedDateTime = selectDateTimeWindow.SelectedDateTime;

                            buttonBackward.Tag = beginTime.ToUniversalTime();
                            buttonForward.Tag = endTime.ToUniversalTime();
                            textBoxTimeValue.Text = ((DateTime)selectedDateTime).ToString("yy-MM-dd HH:mm");
                            textBoxTimeValue.Tag = $"{productStandard}_{selectedDateTime}";
                        }
                    }
                }
                else if (productFileNames.Count == 1)
                {
                    _selectedFilename = productFileNames[0];
                }

                if (String.IsNullOrEmpty(_selectedFilename) == false && selectedDateTime != null)
                {
                    await LoadHDF5File(productStandard, _selectedFilename, selectedDateTime);
                }
            }
            else if (productFileNames.Count > 1)
            {
                var selectDatasetWindow = new SelectDatasetWindow();
                selectDatasetWindow.Owner = this; 
                selectDatasetWindow.dataGrid.ItemsSource = exchangeSetLoader.DatasetInfoItems;
                selectDatasetWindow.ShowDialog();

                _selectedFilename = selectDatasetWindow.SelectedFilename;
                if (String.IsNullOrEmpty(_selectedFilename) == false)
                {
                    await LoadGMLFile(productStandard, _selectedFilename);
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
        private async Task LoadENCFile(string fileName)
        {
            List<Layer>? nonEncLayers =
                myMapView?.Map?.OperationalLayers.ToList().FindAll(tp => !tp.GetType().ToString().Contains("EncLayer"));
            myMapView?.Map?.OperationalLayers.Clear();
            myMapView?.GraphicsOverlays?.Clear();

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
        private async Task LoadHDF5File(string productStandard, string fileName, DateTime? selectedDateTime)
        {
            this.mainRibbon.Title = $"Viewing {fileName.LastPart(@"\")}";
            if (selectedDateTime != null)
            {
                this.mainRibbon.Title += $"@ {((DateTime)selectedDateTime).ToString("yyyy-MM-dd HH:mm")} UTC";
            }

            _dataPackages.Clear();
            dataGridFeatureProperties.ItemsSource = null;
            treeViewFeatures.Items.Clear();

            Layer? encLayer = myMapView?.Map?.OperationalLayers?.ToList().Find(tp => tp.GetType().ToString().Contains("EncLayer"));

            myMapView.Map.OperationalLayers.Clear();
            if (myMapView.Map.OperationalLayers.Count > 0)
            {
                myMapView.Map.OperationalLayers.RemoveAt(0);
            }
            myMapView.GraphicsOverlays.Clear();
            if (myMapView.GraphicsOverlays.Count > 0)
            {
                myMapView.GraphicsOverlays.RemoveAt(0);
            }

            if (encLayer != null)
            {
                myMapView?.Map?.OperationalLayers?.Add(encLayer);
            }

            var timerStart = DateTime.Now;
            try
            {
                SaveRecentFile(fileName);

                // now find out which codingformat is to be used to determine S111 dataparser
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

                if (selectedDateTime == null)
                {
                    // if there's no selected timeframe, retrieve timeframe from HDF5 file and ask user to select a valid date
                    (DateTime start, DateTime end) timeframePresentInFile = await productSupport.RetrieveTimeFrameFromHdfDatasetAsync(fileName);

                    if (timeframePresentInFile.start != DateTime.MinValue &&
                        timeframePresentInFile.end != DateTime.MinValue)
                    {
                        var selectDateTimeWindow = new SelectDateTimeWindow();
                        selectDateTimeWindow.Owner = this;
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
                        textBoxTimeValue.Tag = $"{productStandard}_{selectedDateTime}";
                    }
                }

                _syncContext?.Post(new SendOrPostCallback(txt =>
                {
                    labelStatus.Content = $"Loading {txt} ..";

                }), fileName);

                var dataPackage = await dataPackageParser.ParseAsync(fileName, selectedDateTime).ConfigureAwait(false);
                if (dataPackage != null && dataPackage.GeoFeatures != null && dataPackage.GeoFeatures.Count() > 0)
                {
                    // now process data for display in ESRI ArcGIS viewmodel
                    if (dataPackage.Type == S1xxTypes.Null)
                    {
                        MessageBox.Show($"File '{fileName}' currently can't be rendered. No DataParser is able to render the information present in the file!", "Invalid datapackageparser", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        _syncContext?.Post(new SendOrPostCallback(async o =>
                        {
                            if (string.IsNullOrEmpty(buttonBackward.Tag.ToString()) == false)
                            {
                                if (DateTime.TryParse(buttonBackward.Tag.ToString(), out DateTime beginTime))
                                {
                                    buttonBackward.IsEnabled = selectedDateTime > beginTime;
                                }
                            }

                            if (string.IsNullOrEmpty(buttonForward.Tag.ToString()) == false)
                            {
                                if (DateTime.TryParse(buttonForward.Tag.ToString(), out DateTime endTime))
                                {
                                    buttonForward.IsEnabled = selectedDateTime < endTime;
                                }
                            }

                            if (o != null)
                            {
                                CreateFeatureCollection((IS1xxDataPackage)o);
                            }
                        }), dataPackage);

                        _dataPackages.Add(dataPackage);
                    }
                }
                else if (dataPackage != null && dataPackage.RawHdfData == null && dataPackage.RawXmlData == null)
                {
                    MessageBox.Show($"No datapackageparser exists for '{fileName}' and datacodingformat '{dataCodingFormat}'. Can't display data!", "No datapackageparser", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"File '{fileName}' currently can't be rendered. No data found for selection!", "No data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                var elapsedTime = (DateTime.Now - timerStart).ToString();
                _syncContext?.Post(new SendOrPostCallback(o =>
                {
                    if (o != null)
                    {
                        labelStatus.Content = $"Load time: {o ?? ""} seconds ..";
                        progressBar.Value = 0;

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
            myMapView?.Map?.OperationalLayers?.Clear();
            myMapView?.GraphicsOverlays?.Clear();

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
                        labelStatus.Content = $"Load time: {txt} seconds ..";
                        progressBar.Value = 0;

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
                var newMenuItem = new MenuItem
                {
                    Name = $"MenuItem{i++}",
                    Header = newFileName.LastPart(@"\"),
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

        #region Methods that have connections to ArcGIS

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
                    _resetViewpoint = false;
                }
            }

            // set projection to the srs of the first valid geometry
            int i = 0;
            while (dataPackage.GeoFeatures[i++].Geometry == null && i < dataPackage.GeoFeatures.Length) ;
            SpatialReference? horizontalCRS = SpatialReferences.Wgs84;
            if (i < dataPackage.GeoFeatures.Length)
            {
                horizontalCRS = dataPackage.GeoFeatures[i].Geometry.SpatialReference;
            }

            var featureRendererFactory = _container.Resolve<IFeatureCollectionFactory>();
            if (featureRendererFactory == null)
            {
                throw new Exception("Can't retrieve FeatureRendererFactory!");
            }

            featureRendererFactory.Clear();

            var polygonsTable = featureRendererFactory.Create("PolygonFeatures", polyFields, GeometryType.Polygon, horizontalCRS);
            var linesTable = featureRendererFactory.Create("LineFeatures", lineFields, GeometryType.Polyline, horizontalCRS);
            var pointsTable = featureRendererFactory.Create("PointFeatures", pointFields, GeometryType.Point, horizontalCRS);
            var vectorsTable = featureRendererFactory.Create("VectorFeatures", pointVectorFields, GeometryType.Point, horizontalCRS, true);

            var graphicList = new List<Graphic>();
            var pointFeatureList = new List<Feature>();
            var lineFeatureList = new List<Feature>();
            var polygonFeatureList = new List<Feature>();
            var vectorFeatureList = new List<Feature>();
            var filledPolyFeatureLists = new Dictionary<string, List<Feature>>();

            Parallel.ForEach(dataPackage.GeoFeatures, feature =>
            {
                if (feature is IGeoFeature geoFeature)
                {
                    (string type, Feature feature, Graphic? graphic) renderedFeature = geoFeature.Render(featureRendererFactory, horizontalCRS);

                    lock (this) 
                    {
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
                    }

                    if (renderedFeature.graphic != null)
                    {
                        graphicList.Add(renderedFeature.graphic);
                    }
                }
            });

            await pointsTable.AddFeaturesAsync(pointFeatureList);
            await linesTable.AddFeaturesAsync(lineFeatureList);
            await polygonsTable.AddFeaturesAsync(polygonFeatureList);
            await vectorsTable.AddFeaturesAsync(vectorFeatureList);

            var filledPolygonTables = new List<FeatureCollectionTable>();
            foreach (KeyValuePair<string, List<Feature>> filledPolyFeatureList in filledPolyFeatureLists)
            {
                var filledPolygonTable = featureRendererFactory.Get(filledPolyFeatureList.Key);
                filledPolygonTables.Add(filledPolygonTable);

                filledPolygonTable.AddFeaturesAsync(filledPolyFeatureList.Value);
            }

            var featuresCollection = new FeatureCollection();

            if (filledPolygonTables.Count > 0)
            {
                foreach(FeatureCollectionTable filledPolysTable in filledPolygonTables)
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
                string featureJsonString =
                    @"{
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
                    }";

                var graphicsOverlay = new GraphicsOverlay() { Id = "VectorFeatures" };
                graphicsOverlay.Graphics.AddRange(graphicList);
                myMapView.GraphicsOverlays?.Add(graphicsOverlay);

                // Create a label definition from the JSON string. 
                var idLabelDefinition = LabelDefinition.FromJson(featureJsonString);

                var collectionLayer = new FeatureCollectionLayer(featuresCollection);
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

                            if (layer.FullExtent != null)
                            {
                                datasetExtents.Add(layer.FullExtent);
                            }
                        }

                        if(_resetViewpoint == true)
                        {
                            var fullExtent = GeometryEngine.CombineExtents(datasetExtents);
                            await myMapView.SetViewpointAsync(new Viewpoint(fullExtent));
                        }
                    }
                    catch (Exception) { }
                });
                await collectionLayer.LoadAsync();

                // Add the layer to the Map's Operational Layers collection
                myMapView.Map.OperationalLayers.Add(collectionLayer);
                myMapView.GeoViewTapped += OnMapViewTapped;
            }
        }

        /// <summary>
        ///     If the mapview is tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnMapViewTapped(object sender, GeoViewInputEventArgs e)
        {
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
                    var collectionLayer = operationalLayer as FeatureCollectionLayer;
                    if (collectionLayer != null)
                    {
                        IdentifyLayerResult groupLayerResult =
                            await myMapView.IdentifyLayerAsync(collectionLayer, tapScreenPoint, pixelTolerance, returnPopupsOnly, maxResults);

                        if (groupLayerResult.SublayerResults.Count > 0)
                        {
                            // Iterate each set of child layer results.
                            foreach (IdentifyLayerResult subLayerResult in groupLayerResult.SublayerResults)
                            {
                                // clear featureselection in all layers
                                collectionLayer?.Layers.ToList().ForEach(l => l.ClearSelection());

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
                        Header = $"Selected {results.Count} geo-feature{(results.Count == 1 ? "" : "s")}",
                        Tag = "feature_count",
                        IsExpanded = true
                    };
                    treeViewFeatures.Items.Add(parentTreeNode);

                    foreach (KeyValuePair<string, DataTable> result in results)
                    {
                        TreeViewItem treeNode = new TreeViewItem();
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
