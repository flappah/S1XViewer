using HDF5CSharp.DataTypes;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
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
    public class S104DCF3V11DataParser : HdfDataParserBase, IS104DCF3V11DataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;

        private readonly IDatasetReader _datasetReader;
        private readonly IGeometryBuilderFactory _geometryBuilderFactory;

        /// <summary>
        ///     Empty constructor used for injection purposes
        /// </summary>
        /// <param name="datasetReader"></param>
        /// <param name="geometryBuilderFactory"></param>
        /// <param name="productSupport"></param>
        /// <param name="optionsStorage"></param>
        public S104DCF3V11DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS111ProductSupport productSupport, IOptionsStorage optionsStorage)
        {
            _datasetReader = datasetReader;
            _geometryBuilderFactory = geometryBuilderFactory;
            _productSupport = productSupport;
            _optionsStorage = optionsStorage;
        }

        /// <summary>
        ///     Parses specified HDF5 file
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
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

            var dataPackage = new S111DataPackage
            {
                FileName = hdf5FileName,
                Type = S1xxTypes.S104,
                RawHdfData = null
            };

            _syncContext = SynchronizationContext.Current;
            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = await _productSupport.RetrieveHdf5FileAsync(hdf5FileName);
            (long horizontalCRS, string utmZone) = RetrieveHorizontalCRS(hdf5S111Root, hdf5FileName);
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
            string[] axisNamesStrings = new string[0];
            if (axisNameElement != null)
            {
                axisNamesStrings = _datasetReader.ReadStrings(hdf5FileName, axisNameElement.Name).ToArray();
                if (axisNamesStrings != null && axisNamesStrings.Length == 2)
                {
                    if (axisNamesStrings[0].ToUpper().Equals("LATITUDE") || axisNamesStrings[1].ToUpper().Equals("LATITUDE"))
                    {
                        invertLonLat = axisNamesStrings[0].ToUpper().Equals("LATITUDE") && axisNamesStrings[1].ToUpper().Equals("LONGITUDE");
                    }
                    else if (axisNamesStrings[0].ToUpper().Equals("LAT") || axisNamesStrings[1].ToUpper().Equals("LAT"))
                    {
                        invertLonLat = axisNamesStrings[0].ToUpper().Equals("LAT") && axisNamesStrings[1].ToUpper().Equals("LON");
                    }
                    else if (axisNamesStrings[0].ToUpper().Equals("EASTING") || axisNamesStrings[1].ToUpper().Equals("EASTING"))
                    {
                        invertLonLat = axisNamesStrings[0].ToUpper().Equals("NORTHING") && axisNamesStrings[1].ToUpper().Equals("EASTING");
                    }

                    dataPackage.InvertLonLat = invertLonLat;
                }
            }

            // retrieve boundingbox
            var eastBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("eastBoundLongitude");
            var eastBoundLongitude = eastBoundLongitudeAttribute?.Value<double>(0f) ?? 0.0;
            var northBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("northBoundLatitude");
            var northBoundLatitude = northBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var southBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("southBoundLatitude");
            var southBoundLatitude = southBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var westBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("westBoundLongitude");
            var westBoundLongitude = westBoundLongitudeAttribute?.Value<double>(0f) ?? 0f;

            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int)horizontalCRS);

            Hdf5Element? groupFElement = hdf5S111Root.Children.Find(elm => elm.Name.Equals("/Group_F"));
            if (groupFElement == null)
            {
                return dataPackage;
            }

            // retrieve values for undefined data cells
            double nillValueHeight = -9999.0;
            double nillValueTrend = 0;
            var featureMetaInfoElements = _datasetReader.ReadCompound<WaterLevelInformationInstance>(hdf5FileName, "/Group_F/WaterLevel");
            if (featureMetaInfoElements.values.Length > 0)
            {
                foreach (var featureMetainfoElementValue in featureMetaInfoElements.values)
                {
                    if (featureMetainfoElementValue.code.Equals("waterLevelHeight"))
                    {
                        if (float.TryParse(featureMetainfoElementValue.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out float heightFillValue))
                        {
                            nillValueHeight = heightFillValue;
                        }
                    }
                    else if (featureMetainfoElementValue.code.Equals("waterLevelTrend"))
                    {
                        if (float.TryParse(featureMetainfoElementValue.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out float trendFillValue))
                        {
                            nillValueTrend = trendFillValue;
                        }
                    }
                }
            }

            Hdf5Element? minGroup = _productSupport.FindGroupByDateTime(featureElement.Children, selectedDateTime);
            if (minGroup != null)
            {
                // now retrieve positions 
                var positioningElement = ((Hdf5Element)minGroup.Parent).Children.Find(nd => nd.Name.LastPart("/") == "Positioning");
                if (positioningElement != null)
                {
                    GeometryValueInstance[] positionValues =
                        _datasetReader.Read<GeometryValueInstance>(hdf5FileName, positioningElement.Children[0].Name).ToArray();

                    if (positionValues == null || positionValues.Length == 0)
                    {
                        throw new Exception($"WaterLevel feature with name {positioningElement.Children[0].Name} contains no positions!");
                    }

                    if (String.IsNullOrEmpty(utmZone) == false)
                    {
                        for(int i = 0; i < positionValues.Length; i++)
                        {
                            GeoAPI.Geometries.Coordinate transformedCoordinate =
                                TransformUTMToWGS84(new GeoAPI.Geometries.Coordinate(positionValues[i].longitude, positionValues[i].latitude), utmZone);

                            positionValues[i].longitude = transformedCoordinate.X;
                            positionValues[i].latitude = transformedCoordinate.Y;
                        }
                    }

                    // now retrieve group based on selectedTime 
                    var selectedHdf5Group = _productSupport.FindGroupByDateTime(hdf5S111Root.Children[1].Children, selectedDateTime);
                    if (selectedHdf5Group != null)
                    {
                        // retrieve directions and current speeds
                        var waterLevelInfos =
                            _datasetReader.Read<WaterLevelInstance>(hdf5FileName, selectedHdf5Group.Children[0].Name).ToArray();

                        if (waterLevelInfos.Length != positionValues.Length)
                        {
                            throw new Exception("Positioning information does not match the number of surface current info items!");
                        }

                        var timePointElement = selectedHdf5Group.Attributes.Find("timePoint");
                        var timePointString = timePointElement?.Value<string>() ?? string.Empty;
                        if (DateTime.TryParseExact(timePointString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timePoint))
                        {
                            timePoint = timePoint.ToUniversalTime();
                        }

                        var geoFeatures = new List<IGeoFeature>();

                        await Task.Run(() =>
                        {
                            // build up features and wrap 'em in data package
                            for (int i = 0; i < waterLevelInfos.Length; i++)
                            {
                                var height = waterLevelInfos[i].height;
                                var trend = waterLevelInfos[i].trend;

                                if (height != nillValueHeight)
                                {
                                    var geometry =
                                        _geometryBuilderFactory.Create("Point", new double[] { positionValues[i].longitude }, new double[] { positionValues[i].latitude }, (int)horizontalCRS);

                                    var tidalStationInstance = new TidalStation()
                                    {
                                        Id = selectedHdf5Group.Name + $"_{i}",
                                        FeatureName = new FeatureName[] { new FeatureName { DisplayName = selectedHdf5Group.Name + $"_{i}" } },
                                        TidalHeights = new Dictionary<string, string>(),
                                        TidalTrends = new Dictionary<string, string>(),
                                        SelectedDateTime = timePoint.ToString("ddMMMyyyy HHmm"),
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

                                    geoFeatures.Add(tidalStationInstance);
                                }
                            }

                            var parentElement = selectedHdf5Group.Parent;
                            if (parentElement != null)
                            {
                                int stationNumber = 0;
                                int maxCount = ((Hdf5Element)parentElement).Children.Count;

                                foreach (Hdf5Element? timeValueHdf5Group in parentElement.GetChildren())
                                {
                                    if (timeValueHdf5Group != null && timeValueHdf5Group.Name.Contains("Group_"))
                                    {
                                        var timePointAttribute = timeValueHdf5Group.Attributes.Find("timePoint");
                                        string timePointString = timePointAttribute?.Value<string>("") ?? "";
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

                                    var ratio = 50 + (int)((50.0 / (double)maxCount) * (double)stationNumber++);
                                    _syncContext?.Post(new SendOrPostCallback(r =>
                                    {
                                        Progress?.Invoke((int)r);

                                    }), ratio);
                                }
                            }

                        }).ConfigureAwait(false);

                        dataPackage.InvertLonLat = invertLonLat;
                        dataPackage.RawHdfData = hdf5S111Root;
                        if (geoFeatures.Count > 0)
                        {
                            dataPackage.GeoFeatures = geoFeatures.ToArray();
                            dataPackage.MetaFeatures = new IMetaFeature[0];
                            dataPackage.InformationFeatures = new IInformationFeature[0];
                        }
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
        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            return ParseAsync(hdf5FileName, selectedDateTime).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     No implementation!
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
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
