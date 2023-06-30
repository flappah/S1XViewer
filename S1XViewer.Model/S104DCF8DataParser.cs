using HDF5CSharp.DataTypes;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Geometry;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Features;
using S1XViewer.Types.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Model
{
    public class S104DCF8DataParser : HdfDataParserBase, IS104DCF8DataParser
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
        public S104DCF8DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS104ProductSupport productSupport, IOptionsStorage optionsStorage)
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
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime)
        {
            if (string.IsNullOrEmpty(hdf5FileName))
            {
                throw new ArgumentException($"'{nameof(hdf5FileName)}' cannot be null or empty.", nameof(hdf5FileName));
            }

            if (selectedDateTime == null)
            {
                return new S111DataPackage
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
                    invertLonLat = axisNamesStrings[0].ToUpper().Equals("LATITUDE") && axisNamesStrings[1].ToUpper().Equals("LONGITUDE");
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
            if (selectedWaterLevelFeatureElement != null)
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
                        int stationNumber = 0;
                        int maxCount = selectedWaterLevelFeatureElement.Children.Count;
                        foreach (Hdf5Element? groupHdf5Group in selectedWaterLevelFeatureElement.Children)
                        {
                            if (groupHdf5Group.Name.Contains("Group_"))
                            {
                                var startDateTimeAttribute = groupHdf5Group.Attributes.Find("startDateTime");
                                string startDateTimeString = startDateTimeAttribute?.Value<string>("") ?? "";
                                DateTime startDateTime =
                                    DateTime.ParseExact(startDateTimeString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();

                                var featureNameAttribute = groupHdf5Group.Attributes.Find("stationName");
                                var featureName = featureNameAttribute?.Value<string>("") ?? "";

                                var timeRecordIntervalAttribute = groupHdf5Group.Attributes.Find("timeRecordInterval");
                                var timeRecordInterval = timeRecordIntervalAttribute?.Value<long>(0) ?? 0;

                                Int32 index = -1;
                                var heights = new Dictionary<string, string>();
                                var trends = new Dictionary<string, string>();
                                if (timeRecordInterval > 0)
                                {
                                    var waterLevelInfos =
                                        _datasetReader.Read<WaterLevelInstance>(hdf5FileName, groupHdf5Group.Children[0].Name).ToArray();

                                    if (waterLevelInfos != null)
                                    {
                                        // build up features ard wrap 'em in a data package
                                        index = (Int32)(((TimeSpan)(selectedDateTime - startDateTime)).TotalSeconds / timeRecordInterval);
                                        /*  
                                         * with varying group sizes indexes can sometimes fall outside the boundary because different 
                                         * reference tidal stations have timeseries of different length wrapped inside 1 package. 
                                         * Set to limits if necessary
                                         */
                                        if (index < 0)
                                        {
                                            index = 0;
                                        }
                                        else if (index >= waterLevelInfos.Length)
                                        {
                                            index = waterLevelInfos.Length - 1;
                                        }

                                        short i = 0;
                                        foreach (WaterLevelInstance waterlevelInfo in waterLevelInfos)
                                        {
                                            heights.Add(startDateTime.AddSeconds(timeRecordInterval * i).ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), waterlevelInfo.height.ToString().Replace(",", "."));
                                            trends.Add(startDateTime.AddSeconds(timeRecordInterval * i).ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), waterlevelInfo.trend.ToString());
                                            i++;
                                        }
                                    }
                                }
                                else
                                {
                                    var waterLevelWithTimeInfos =
                                        _datasetReader.ReadCompound<WaterLevelWithTimeInstance>(hdf5FileName, groupHdf5Group.Children[0].Name).values.ToArray();

                                    if (waterLevelWithTimeInfos != null)
                                    {   
                                        short i = 0;
                                        DateTime previousDateTime = DateTime.MinValue;
                                        foreach (WaterLevelWithTimeInstance waterlevelWithTimeInfo in waterLevelWithTimeInfos)
                                        {
                                            if (DateTime.TryParseExact(waterlevelWithTimeInfo.waterLevelTime, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
                                            {
                                                heights.Add(parsedDateTime.ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), waterlevelWithTimeInfo.waterLevelHeight.ToString().Replace(",", "."));
                                                trends.Add(parsedDateTime.ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), waterlevelWithTimeInfo.waterLevelTrend.ToString());

                                                if (((DateTime)selectedDateTime).Between(previousDateTime, parsedDateTime))
                                                {
                                                    index = i;
                                                }

                                                previousDateTime = parsedDateTime;
                                                i++;
                                            }
                                        }
                                    }
                                }

                                Esri.ArcGISRuntime.Geometry.Geometry geometry =
                                    _geometryBuilderFactory.Create("Point", new double[] { positionValues[stationNumber].longitude }, new double[] { positionValues[stationNumber].latitude }, (int)horizontalCRS);

                                var tidalStationInstance = new TidalStation()
                                {
                                    Id = groupHdf5Group.Name,
                                    FeatureName = new FeatureName[] { new FeatureName { DisplayName = featureName } },
                                    TidalHeights = heights,
                                    TidalTrends = trends,
                                    SelectedIndex = (short)index,
                                    SelectedDateTime = heights.ElementAt(index).Key,
                                    SelectedHeight = heights.ElementAt(index).Value + " m",
                                    SelectedTrend = trends.ElementAt(index).Value switch
                                    {
                                        "1" => "decreasing",
                                        "2" => "increasing",
                                        "3" => "steady",
                                        _ => "unknown"
                                    },
                                    Geometry = geometry
                                };
                                geoFeatures[stationNumber] = tidalStationInstance;
                                stationNumber++;
                            }

                            var ratio = 50 + (int)((50.0 / (double)maxCount) * (double)stationNumber);
                            _syncContext?.Post(new SendOrPostCallback(r =>
                            {
                                Progress?.Invoke((int)r);

                            }), ratio);
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
        /// 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            throw new NotImplementedException();
        }

    }
}
