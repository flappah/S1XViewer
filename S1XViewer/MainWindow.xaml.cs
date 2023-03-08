using Autofac;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Hydrography;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
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
using System.Xml;
using static S1XViewer.Model.Interfaces.IDataParser;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Autofac.IContainer _container;
        private List<IS1xxDataPackage> _dataPackages = new List<IS1xxDataPackage>();
        private SynchronizationContext? _syncContext;

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
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        var newMenuItem = new MenuItem
                        {
                            Name = $"MenuItem{i++}",
                            Header = fileName
                        };
                        newMenuItem.Click += AutoOpen_Click;

                        RecentFilesMenuItem.Items.Add(newMenuItem);
                    });
                }
            });

            myMapView.Map = new Map(BasemapStyle.ArcGISOceans);

            myMapView.SetViewpoint(new Viewpoint(
                latitude: 52,
                longitude: 3,               
                scale: 3000000));
            
        }

        #region Menu Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public void AppExit_Click(object obj, EventArgs e)
        {
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
                Filter = "XML/GML files (*.xml;*.gml)|*.xml;*.gml|HDF5 files (*.h5;*.hdf5)|*.h5;*.hdf5|ENC files (*.000)|*.031|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var currentFolder = openFileDialog.FileName.Substring(0, openFileDialog.FileName.LastIndexOf(@"\"));
                if (String.IsNullOrEmpty(currentFolder) == false)
                {
                    Directory.SetCurrentDirectory(currentFolder);
                }

                var fileName = openFileDialog.FileName;
                
                if (fileName.ToUpper().Contains("CATALOG") && fileName.ToUpper().Contains(".XML"))
                {
                    LoadExchangeSet(fileName);
                }
                else if (fileName.ToUpper().Contains(".XML") || fileName.ToUpper().Contains(".GML"))
                {
                    LoadGMLFile("", fileName);
                }
                else if (fileName.ToUpper().Contains(".H5") || fileName.ToUpper().Contains(".HDF5")) 
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
                        var selectStandardForm = new SelectStandardForm();
                        selectStandardForm.ShowDialog();
                        productStandard = selectStandardForm.SelectedStandard;
                    }
                    else
                    {
                        productStandard = $"S{productStandard}";
                    }

                    LoadHDF5File(productStandard, fileName, null);
                }
                else if (fileName.Contains(".031"))
                {
                    LoadENCFile(fileName);
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AutoOpen_Click(object sender, RoutedEventArgs e)
        {
            var fileName = ((MenuItem)sender).Header.ToString();
            if (fileName?.ToLower().Contains(".xml") == true|| fileName?.ToLower().Contains(".gml") == true)
            {
                LoadGMLFile("", fileName);
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
                string selectedFileName = string.Empty;
                DateTime? selectedDateTime = DateTime.Now;
                if (productFileNames.Count > 1)
                {
                    var selectDatasetWindow = new SelectDatasetWindow();
                    selectDatasetWindow.dataGrid.ItemsSource = exchangeSetLoader.DatasetInfoItems;
                    selectDatasetWindow.ShowDialog();
                    selectedFileName = selectDatasetWindow.SelectedFilename;

                    var filename = selectedFileName.LastPart(@"\");
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
                            selectDateTimeWindow.textblockInfo.Text = $"Values available from {beginTime.ToUniversalTime().ToString()} UTC to {endTime.ToUniversalTime().ToString()} UTC. Select a Date and a Time.";
                            selectDateTimeWindow.datePicker.DisplayDateStart = beginTime.ToUniversalTime();
                            selectDateTimeWindow.datePicker.DisplayDateEnd = endTime.ToUniversalTime();
                            selectDateTimeWindow.ShowDialog();
                            selectedDateTime = selectDateTimeWindow.SelectedDateTime;
                        }
                    }
                }
                else if (productFileNames.Count == 1)
                {
                    selectedFileName = productFileNames[0];
                }

                if (String.IsNullOrEmpty(selectedFileName) == false && selectedDateTime != null)
                {
                    await LoadHDF5File(productStandard, selectedFileName, selectedDateTime);
                }
            }
            else if (productFileNames.Count > 1)
            {
                var selectDatasetWindow = new SelectDatasetWindow();
                selectDatasetWindow.dataGrid.ItemsSource = exchangeSetLoader.DatasetInfoItems;
                selectDatasetWindow.ShowDialog();

                var selectedFileName = selectDatasetWindow.SelectedFilename;
                if (String.IsNullOrEmpty(selectedFileName) == false)
                {
                    await LoadGMLFile(productStandard, selectedFileName);
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
            Title = $"IHO S1xx standard Viewer ({fileName.LastPart(@"\")})";
            _dataPackages.Clear();
            dataGridFeatureProperties.ItemsSource = null;
            treeViewFeatures.Items.Clear();

            Layer? encLayer = myMapView?.Map?.OperationalLayers?.ToList().Find(tp => tp.GetType().ToString().Contains("EncLayer"));
            myMapView?.Map?.OperationalLayers?.Clear();

            if (encLayer != null)
            {
                myMapView?.Map?.OperationalLayers?.Add(encLayer);
            }

            try
            {
                _syncContext?.Post(new SendOrPostCallback(txt =>
                {
                    labelStatus.Content = $"Loading {txt} ..";

                }), fileName);

                var datetimeStart = DateTime.Now;

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

                var dataPackage = await dataPackageParser.ParseAsync(fileName, selectedDateTime).ConfigureAwait(false);
                var elapsedTime = (DateTime.Now - datetimeStart).ToString();

                _syncContext?.Post(new SendOrPostCallback(txt =>
                {
                    if (txt != null)
                    {
                        labelStatus.Content = $"Load time: {txt} seconds ..";
                        progressBar.Value = 0;
                    }
                }), elapsedTime);

                // now process data for display in ESRI ArcGIS viewmodel





            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
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
        }

        /// <summary>
        ///     Loads the specified GML file
        /// </summary>
        /// <param name="productStandard">standard to process data on</param>
        /// <param name="fileName">fileName</param>
        private async Task LoadGMLFile(string productStandard, string fileName)
        {
            Title = $"IHO S1xx standard Viewer ({fileName.LastPart(@"\")})";
            _dataPackages.Clear();
            dataGridFeatureProperties.ItemsSource = null;
            treeViewFeatures.Items.Clear();

            Layer? encLayer = myMapView?.Map?.OperationalLayers?.ToList().Find(tp => tp.GetType().ToString().Contains("EncLayer"));
            myMapView?.Map?.OperationalLayers?.Clear();

            if (encLayer != null)
            {
                myMapView?.Map?.OperationalLayers?.Add(encLayer);
            }

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

                var datetimeStart = DateTime.Now;
                var dataPackage = await dataPackageParser.ParseAsync(xmlDoc).ConfigureAwait(false);
                var elapsedTime = (DateTime.Now - datetimeStart).ToString();

                _syncContext?.Post(new SendOrPostCallback(txt =>
                {
                    if (txt != null)
                    {
                        labelStatus.Content = $"Load time: {txt} seconds ..";
                    }
                }), elapsedTime);

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
                    Header = newFileName
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
            string theJSON_String =
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

            // Create a label definition from the JSON string. 
            LabelDefinition idLabelDefinition = LabelDefinition.FromJson(theJSON_String);

            List<Field> polyFields = new List<Field>();
            Field idField = new Field(FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(FieldType.Text, "FeatureName", "Name", 255);
            polyFields.Add(idField);
            polyFields.Add(nameField);

            List<Field> pointFields = new List<Field>
            {
                idField,
                nameField
            };

            List<Field> lineFields = new List<Field>
            {
                idField,
                nameField
            };

            FeatureCollectionTable polysTable = new FeatureCollectionTable(polyFields, GeometryType.Polygon, SpatialReferences.Wgs84)
            {
                Renderer = CreateRenderer(GeometryType.Polygon),
                DisplayName = "Polygons"
            };

            FeatureCollectionTable linesTable = new FeatureCollectionTable(lineFields, GeometryType.Polyline, SpatialReferences.Wgs84)
            {
                Renderer = CreateRenderer(GeometryType.Polyline),
                DisplayName = "Lines"
            };

            FeatureCollectionTable pointTable = new FeatureCollectionTable(pointFields, GeometryType.Point, SpatialReferences.Wgs84)
            {
                Renderer = CreateRenderer(GeometryType.Point),
                DisplayName = "Points"
            };

            foreach (IFeature feature in dataPackage.GeoFeatures)
            {
                if (feature is IGeoFeature geoFeature)
                {
                    if (geoFeature.Geometry is Esri.ArcGISRuntime.Geometry.MapPoint)
                    {
                        Feature pointFeature = pointTable.CreateFeature();
                        pointFeature.SetAttributeValue(idField, feature.Id);
                        pointFeature.SetAttributeValue(nameField, geoFeature.FeatureName?.First()?.Name);
                        pointFeature.Geometry = geoFeature.Geometry;

                        await pointTable.AddFeatureAsync(pointFeature);
                    }
                    else if (geoFeature.Geometry is Esri.ArcGISRuntime.Geometry.Polyline)
                    {
                        Feature lineFeature = linesTable.CreateFeature();
                        lineFeature.SetAttributeValue(idField, feature.Id);
                        lineFeature.SetAttributeValue(nameField, geoFeature.FeatureName?.First()?.Name);
                        lineFeature.Geometry = geoFeature.Geometry;

                        await linesTable.AddFeatureAsync(lineFeature);
                    }
                    else
                    {
                        Feature polyFeature = polysTable.CreateFeature();
                        polyFeature.SetAttributeValue(idField, feature.Id);
                        polyFeature.SetAttributeValue(nameField, geoFeature.FeatureName?.First()?.Name);
                        polyFeature.Geometry = geoFeature.Geometry;

                        await polysTable.AddFeatureAsync(polyFeature);
                    }
                }
            }

            FeatureCollection featuresCollection = new FeatureCollection();
            featuresCollection.Tables.Clear();

            if (pointTable.Count() > 0)
            {
                featuresCollection.Tables.Add(pointTable);
            }
            if (polysTable.Count() > 0)
            {
                featuresCollection.Tables.Add(polysTable);
            }
            if (linesTable.Count() > 0)
            {
                featuresCollection.Tables.Add(linesTable);
            }

            var collectionLayer = new FeatureCollectionLayer(featuresCollection);

            // When the layer loads, zoom the map view to the extent of the feature collection
            collectionLayer.Loaded += (s, e) => Dispatcher.Invoke(() =>
            {
                try
                {
                    foreach (FeatureLayer layer in collectionLayer.Layers)
                    {
                        layer.LabelDefinitions.Add(idLabelDefinition);
                        layer.LabelsEnabled = true;

                        myMapView.SetViewpointAsync(new Viewpoint(layer.FullExtent));
                    }
                }
                catch (Exception) { }
            });

            // Add the layer to the Map's Operational Layers collection
            myMapView.Map.OperationalLayers.Add(collectionLayer);
            myMapView.GeoViewTapped += OnMapViewTapped;
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
                                    if (layer != null)
                                    {
                                        layer.SelectFeature(idFeature);
                                    }

                                    if (idElement.Attributes.ContainsKey("FeatureId"))
                                    {
                                        if (!String.IsNullOrEmpty(idElement.Attributes["FeatureId"]?.ToString()))
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

        /// <summary>
        ///     Creates the renderer for features on the map
        /// </summary>
        /// <param name="rendererType"></param>
        /// <returns></returns>
        private Renderer CreateRenderer(GeometryType rendererType)
        {
            // Return a simple renderer to match the geometry type provided
            Symbol sym = null;

            switch (rendererType)
            {
                case GeometryType.Point:
                case GeometryType.Multipoint:
                    // Create a marker symbol
                    sym = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.X, System.Drawing.Color.Red, 10);

                    break;
                case GeometryType.Polyline:
                    // Create a line symbol
                    sym = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.DarkGray, 3);

                    break;
                case GeometryType.Polygon:
                    // Create a fill symbol
                    var lineSym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 50, 50, 50), 1);
                    sym = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(25, System.Drawing.Color.LightGray), lineSym);

                    break;
                default:
                    break;
            }

            // Return a new renderer that uses the symbol created above
            return new SimpleRenderer(sym);
        }

        #endregion

    }
}
