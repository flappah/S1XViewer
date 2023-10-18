using HDF5CSharp.DataTypes;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Features;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Automation.Peers;

namespace S1XViewer.Model
{
    public class S104DCF1DataParser : HdfDataParserBase, IS104DCF1DataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;

        private readonly IDatasetReader _datasetReader;
        private readonly IGeometryBuilderFactory _geometryBuilderFactory;
        private readonly IOptionsStorage _optionsStorage;

        /// <summary>
        ///     Empty constructor used for injection purposes
        /// </summary>
        /// <param name="datasetReader"></param>
        /// <param name="geometryBuilderFactory"></param>
        /// <param name="productSupport"></param>
        /// <param name="optionsStorage"></param>
        public S104DCF1DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS104ProductSupport productSupport, IOptionsStorage optionsStorage)
        {
            _datasetReader = datasetReader;
            _geometryBuilderFactory = geometryBuilderFactory;
            _productSupport = productSupport;
            _optionsStorage = optionsStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async override Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime)
        {
            if (string.IsNullOrEmpty(hdf5FileName))
            {
                throw new ArgumentException($"'{nameof(hdf5FileName)}' cannot be null or empty.", nameof(hdf5FileName));
            }

            if (selectedDateTime == null)
            {
                return new S104DataPackage
                {
                    Type = S1xxTypes.Null,
                    RawHdfData = null,
                    GeoFeatures = new IGeoFeature[0],
                    MetaFeatures = new IMetaFeature[0],
                    InformationFeatures = new IInformationFeature[0]
                };
            }

            var dataPackage = new S104DataPackage
            {
                FileName = hdf5FileName,
                Type = S1xxTypes.S104,
                RawHdfData = null
            };

            _syncContext = SynchronizationContext.Current;
            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = await _productSupport.RetrieveHdf5FileAsync(hdf5FileName);
            long horizontalCRS = RetrieveHorizontalCRS(hdf5S111Root, hdf5FileName);
            if (horizontalCRS <= 0)
            {
                string defaultCRSString = _optionsStorage.Retrieve("comboBoxCRS");
                if (long.TryParse(defaultCRSString, out horizontalCRS) == false)
                {
                    horizontalCRS = 4326; // wgs84
                }
            }
            dataPackage.DefaultCRS = (int)horizontalCRS;
            _geometryBuilderFactory.DefaultCRS = horizontalCRS.ToString();

            string invertLonLatString = _optionsStorage.Retrieve("checkBoxInvertLonLat");
            if (bool.TryParse(invertLonLatString, out bool invertLonLat) == false)
            {
                invertLonLat = false;
            }
            dataPackage.InvertLonLat = invertLonLat;
            _geometryBuilderFactory.InvertLonLat = invertLonLat;

            // retrieve relevant time-frame from SurfaceCurrents collection
            Hdf5Element? featureElement = hdf5S111Root.Children.Find(elm => elm.Name.Equals("/WaterLevel"));
            if (featureElement == null)
            {
                return dataPackage;
            }

            // check position ordering lon/lat vs lat/lon
            var axisNameElement = featureElement.Children.Find(elm => elm.Name.Equals("/WaterLevel/axisNames"));
            if (axisNameElement != null)
            {
                var axisNamesStrings = _datasetReader.ReadStrings(hdf5FileName, axisNameElement.Name).ToArray();
                if (axisNamesStrings != null && axisNamesStrings.Length == 2)
                {
                    invertLonLat = axisNamesStrings[0].ToUpper().Contains("LAT") && axisNamesStrings[1].ToUpper().Contains("LON");
                    dataPackage.InvertLonLat = invertLonLat;
                }
            }

            // retrieve boundingbox
            var eastBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("eastBoundLongitude");
            var eastBoundLongitude = eastBoundLongitudeAttribute?.Value<double>(0.0) ?? 0.0;
            var northBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("northBoundLatitude");
            var northBoundLatitude = northBoundLatitudeAttribute?.Value<double>(0.0) ?? 0.0;
            var southBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("southBoundLatitude");
            var southBoundLatitude = southBoundLatitudeAttribute?.Value<double>(0.0) ?? 0.0;
            var westBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("westBoundLongitude");
            var westBoundLongitude = westBoundLongitudeAttribute?.Value<double>(0.0) ?? 0.0;

            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int)horizontalCRS);

            var selectedWaterLevelFeatureElement = _productSupport.FindFeatureByDateTime(featureElement.Children, selectedDateTime);
            if (selectedWaterLevelFeatureElement != null && selectedWaterLevelFeatureElement.Children.Count() > 0)
            {
                // now retrieve positions 
                var positioningElement = selectedWaterLevelFeatureElement.Children.Find(nd => nd.Name.LastPart("/") == "Positioning");
                if (positioningElement != null)
                {
                    var positionValues =
                        _datasetReader.Read<GeometryValueInstance>(hdf5FileName, positioningElement.Children[0].Name).ToArray();

                    if (positionValues == null || positionValues.Length == 0)
                    {
                        throw new Exception($"WaterLevel with name {positioningElement.Children[0].Name} contains no positions!");
                    }

                    // retrieve directions and current speeds
                    var numberOfStationsAttribute = selectedWaterLevelFeatureElement.Attributes.Find("numberOfStations");
                    var numberOfStations = numberOfStationsAttribute?.Value<long>(0) ?? 0;
                    if (numberOfStations != positionValues.Count())
                    {
                        throw new Exception("Insufficient number of position values!");
                    }

                    IGeoFeature[] geoFeatures = new IGeoFeature[numberOfStations];

                    await Task.Run(() =>
                    {
                        TimeSpan minTimeSpan = TimeSpan.MaxValue;
                        Hdf5Element? selectedHdf5Group = null;
                        DateTime selectedTimePoint = DateTime.MinValue;
                        DateTime timePoint = DateTime.MinValue;

                        foreach (Hdf5Element? groupHdf5Group in selectedWaterLevelFeatureElement.Children)
                        {
                            // find out which Group_x to use based on the selected time value
                            if (groupHdf5Group.Name.Contains("Group_"))
                            {
                                var timePointAttribute = groupHdf5Group.Attributes.Find("timePoint");
                                if (timePointAttribute != null)
                                {
                                    try
                                    {
                                        string timePointString = timePointAttribute?.Value<string>("") ?? "";
                                        timePoint =
                                            DateTime.ParseExact(timePointString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();

                                        TimeSpan? differenceTimeSpan = selectedDateTime - timePoint;
                                        if (differenceTimeSpan != null)
                                        {
                                            if (Math.Abs(differenceTimeSpan.Value.TotalSeconds) < Math.Abs(minTimeSpan.TotalSeconds))
                                            {
                                                minTimeSpan = differenceTimeSpan.Value;
                                                selectedHdf5Group = groupHdf5Group;
                                                selectedTimePoint = timePoint;
                                            }
                                        }
                                    }
                                    catch(Exception) { }
                                }
                            }
                        }

                        // create all features based on selected datetime
                        if (selectedHdf5Group != null && selectedTimePoint != DateTime.MinValue) 
                        {
                            var waterLevelInfos =
                                _datasetReader.Read<WaterLevelInstance>(hdf5FileName, selectedHdf5Group.Children[0].Name).ToArray();

                            for (int index = 0; index < waterLevelInfos.Length; index++)
                            {
                                // build up features ard wrap 'em in data package
                                float height = waterLevelInfos[index].height;
                                short trend = waterLevelInfos[index].trend;

                                Esri.ArcGISRuntime.Geometry.Geometry? geometry =
                                    _geometryBuilderFactory.Create("Point", new double[] { positionValues[index].longitude }, new double[] { positionValues[index].latitude }, (int)horizontalCRS);

                                var tidalStationInstance = new TidalStation()
                                {
                                    Id = $"{selectedHdf5Group.Name}_{index}",
                                    FeatureName = new FeatureName[] { new FeatureName { DisplayName = $"{selectedHdf5Group.Name}_{index}" } },
                                    TidalHeights = new Dictionary<string, string>(),
                                    TidalTrends = new Dictionary<string, string>(),
                                    SelectedIndex = (short)index,
                                    SelectedDateTime = selectedTimePoint.ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")),
                                    SelectedHeight = Math.Round(height, 2).ToString().Replace(",", ".") + " m",
                                    SelectedTrend = trend.ToString() switch
                                    {
                                        "1" => "decreasing",
                                        "2" => "increasing",
                                        "3" => "steady",
                                        _ => "unknown"
                                    },
                                    Geometry = geometry
                                };
                                geoFeatures[index] = tidalStationInstance;
                            }

                            // now retrieve all height- and trend series
                            var parentGroup = selectedHdf5Group.Parent;
                            if (parentGroup != null)
                            {
                                int stationNumber = 0;
                                int maxCount = selectedWaterLevelFeatureElement.Children.Count;

                                foreach (Hdf5Element? timeValueHdf5Group in parentGroup.GetChildren())
                                {
                                    if (timeValueHdf5Group != null && timeValueHdf5Group.Name.Contains("Group_"))
                                    {
                                        Hdf5AttributeElement? timePointAttribute = timeValueHdf5Group.Attributes.Find("timePoint");
                                        if (timePointAttribute != null)
                                        {
                                            string timePointString = timePointAttribute.Value<string>("") ?? "";
                                            timePoint =
                                                DateTime.ParseExact(timePointString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();

                                            waterLevelInfos =
                                                _datasetReader.Read<WaterLevelInstance>(hdf5FileName, timeValueHdf5Group.Children[0].Name).ToArray();

                                            for (int index = 0; index < waterLevelInfos.Length; index++)
                                            {
                                                float height = waterLevelInfos[index].height;
                                                short trend = waterLevelInfos[index].trend;

                                                ((TidalStation)geoFeatures[index]).TidalHeights.Add(timePoint.ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), Math.Round(height, 2).ToString().Replace(",", "."));
                                                ((TidalStation)geoFeatures[index]).TidalTrends.Add(timePoint.ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), trend.ToString() switch
                                                {
                                                    "1" => "decreasing",
                                                    "2" => "increasing",
                                                    "3" => "steady",
                                                    _ => "unknown"
                                                });
                                            }
                                        }
                                    }

                                    var ratio = 50 + (int)((50.0 / (double)maxCount) * (double)stationNumber++);
                                    _syncContext?.Post(new SendOrPostCallback(r =>
                                    {
                                        Progress?.Invoke((int)r);

                                    }), ratio);
                                }
                            }
                        }
                    });

                    dataPackage.RawHdfData = hdf5S111Root;
                    if (geoFeatures.Length > 0)
                    {
                        dataPackage.GeoFeatures = geoFeatures.ToArray();
                        dataPackage.MetaFeatures = new IMetaFeature[0];
                        dataPackage.InformationFeatures = new IInformationFeature[0];
                    }
                }
            }

            Progress?.Invoke(100);
            return dataPackage;
        }

        /// <summary>
        ///     Parses specified HDF5 file
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            return new S104DataPackage
            {
                Type = S1xxTypes.Null,
                RawHdfData = null,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }

        /// <summary>
        ///     No implementation!
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            return new S104DataPackage
            {
                Type = S1xxTypes.Null,
                RawHdfData = null,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }

        /// <summary>
        ///     No implementation!
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            return new S104DataPackage
            {
                Type = S1xxTypes.Null,
                RawHdfData = null,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }
    }
}
