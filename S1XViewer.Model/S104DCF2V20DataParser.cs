using HDF5CSharp.DataTypes;
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
using S1XViewer.Base;

namespace S1XViewer.Model
{
    public class S104DCF2V20DataParser : HdfDataParserBase, IS104DCF2V20DataParser
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
        public S104DCF2V20DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS111ProductSupport productSupport, IOptionsStorage optionsStorage)
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
            var eastBoundLongitude = hdf5S111Root.Attributes.Find("eastBoundLongitude")?.Value<double>(0f) ?? 0f;
            var northBoundLatitude = hdf5S111Root.Attributes.Find("northBoundLatitude")?.Value<double>(0f) ?? 0f;
            var southBoundLatitude = hdf5S111Root.Attributes.Find("southBoundLatitude")?.Value<double>(0f) ?? 0f;
            var westBoundLongitude = hdf5S111Root.Attributes.Find("westBoundLongitude")?.Value<double>(0f) ?? 0f;

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

            var geoFeatures = new List<IGeoFeature>();
            await Task.Run(() =>
            {
                var geoFeaturesLookupTable = new Dictionary<string, TidalStation>();
                DateTime oldTimePoint = DateTime.MinValue;

                int max = 0;
                foreach (Hdf5Element? waterLevelFeatureInstanceNode in featureElement.Children)
                {
                    foreach (Hdf5Element? group in waterLevelFeatureInstanceNode.Children)
                    {
                        max++;
                    }
                }

                foreach (Hdf5Element? waterLevelFeatureInstanceNode in featureElement.Children)
                {
                    if (waterLevelFeatureInstanceNode != null && waterLevelFeatureInstanceNode.Name.Contains("/WaterLevel."))
                    {
                        var gridOriginLatitude = waterLevelFeatureInstanceNode.Attributes.Find("gridOriginLatitude")?.Value<double>() ?? -999.0;
                        var gridOriginLongitude = waterLevelFeatureInstanceNode.Attributes.Find("gridOriginLongitude")?.Value<double>() ?? -999.0;
                        var gridSpacingLatitudinal = waterLevelFeatureInstanceNode.Attributes.Find("gridSpacingLatitudinal")?.Value<double>() ?? -999.0;
                        var gridSpacingLongitudinal = waterLevelFeatureInstanceNode.Attributes.Find("gridSpacingLongitudinal")?.Value<double>() ?? -999.0;

                        if (gridOriginLatitude == -999.0 || gridOriginLongitude == -999.0 || gridSpacingLatitudinal == -999.0 || gridSpacingLongitudinal == -999.0)
                        {
                            return;
                        }

                        if (String.IsNullOrEmpty(utmZone) == false)
                        {
                            GeoAPI.Geometries.Coordinate transformedCoordinate =
                                TransformUTMToWGS84(new GeoAPI.Geometries.Coordinate(gridOriginLongitude, gridOriginLatitude), utmZone);

                            gridOriginLongitude = transformedCoordinate.X;
                            gridOriginLatitude = transformedCoordinate.Y;
                        }

                        var numPointsLatitude = waterLevelFeatureInstanceNode.Attributes.Find("numPointsLatitudinal")?.Value<int>() ?? -1;
                        var numPointsLongitude = waterLevelFeatureInstanceNode.Attributes.Find("numPointsLongitudinal")?.Value<int>() ?? -1;

                        if (numPointsLatitude == -1 || numPointsLongitude == -1)
                        {
                            return;
                        }

                        var startSequence = waterLevelFeatureInstanceNode.Attributes.Find("startSequence")?.Value<string>() ?? string.Empty;

                        var surfaceFeatureNode = (Hdf5Element)waterLevelFeatureInstanceNode.Parent;
                        if (surfaceFeatureNode == null)
                        {
                            return;
                        }

                        var scanDirection = surfaceFeatureNode.Attributes.Find("sequencingRule.scanDirection")?.Value<string>() ?? string.Empty;

                        short i = 0;
                        foreach (Hdf5Element? group in waterLevelFeatureInstanceNode.Children)
                        {
                            var timePointElement = group.Attributes.Find("timePoint");
                            var timePointString = timePointElement?.Value<string>() ?? string.Empty;
                            if (DateTime.TryParseExact(timePointString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timePoint))
                            {
                                timePoint = timePoint.ToUniversalTime();

                                var currentDataset =
                                      _datasetReader.ReadArrayOfFloats(hdf5FileName, group.Children[0].Name, numPointsLatitude, numPointsLongitude * 3);

                                if (currentDataset.members.Length == 0)
                                {
                                    return;
                                }

                                int innerLoopMax = numPointsLongitude;
                                int outerLoopMax = numPointsLatitude;
                                for (int outerLoopIdx = 0; outerLoopIdx < outerLoopMax; outerLoopIdx++)
                                {
                                    for (int innerLoopIdx = 0; innerLoopIdx < (innerLoopMax * 3); innerLoopIdx += 3)
                                    {
                                        // build up features and wrap 'em in data package
                                        float trend;
                                        float height;
                                        if (currentDataset.members[0].Equals("waterLevelHeight"))
                                        {
                                            height = currentDataset.values[outerLoopIdx, innerLoopIdx];
                                            trend = currentDataset.values[outerLoopIdx, innerLoopIdx + 1];
                                        }
                                        else
                                        {
                                            trend = currentDataset.values[outerLoopIdx, innerLoopIdx];
                                            height = currentDataset.values[outerLoopIdx, innerLoopIdx + 1];
                                        }

                                        // since trend is being read as a float and the value doesn't convert correctly, do a new determination
                                        trend = trend.ToString() switch
                                        {
                                            "0" => 0,
                                            "1E-45" => 1,
                                            "3E-45" => 2,
                                            "4E-45" => 3,
                                            _ => 0
                                        };

                                        if (height != nillValueHeight)
                                        {
                                            double longitude = gridOriginLongitude + (((double)innerLoopIdx / 3.0) * gridSpacingLongitudinal);
                                            double latitude = gridOriginLatitude + ((double)outerLoopIdx * gridSpacingLatitudinal);

                                            Esri.ArcGISRuntime.Geometry.Geometry? geometry =
                                                _geometryBuilderFactory.Create("Point", new double[] { longitude }, new double[] { latitude }, (int)horizontalCRS);

                                            var selectedHeight = -9999.0f;
                                            short selectedTrend = -1;
                                            if (((DateTime)selectedDateTime).Between(oldTimePoint, timePoint))
                                            {
                                                selectedHeight = height;
                                                selectedTrend = (short)trend;
                                            }

                                            if (geoFeaturesLookupTable.ContainsKey($"{outerLoopIdx}_{innerLoopIdx}"))
                                            {
                                                geoFeaturesLookupTable[$"{outerLoopIdx}_{innerLoopIdx}"].TidalHeights.Add(timePoint.ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), height.ToString().Replace(",", "."));
                                                geoFeaturesLookupTable[$"{outerLoopIdx}_{innerLoopIdx}"].TidalTrends.Add(timePoint.ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), ((short)trend).ToString());

                                                if (geoFeaturesLookupTable[$"{outerLoopIdx}_{innerLoopIdx}"].SelectedHeight == "-9999 m")
                                                {
                                                    geoFeaturesLookupTable[$"{outerLoopIdx}_{innerLoopIdx}"].SelectedIndex = i;
                                                    geoFeaturesLookupTable[$"{outerLoopIdx}_{innerLoopIdx}"].SelectedDateTime = ((DateTime)selectedDateTime).ToString("ddMMMyyyy HHmm");
                                                    geoFeaturesLookupTable[$"{outerLoopIdx}_{innerLoopIdx}"].SelectedHeight = Math.Round(selectedHeight, 2).ToString().Replace(",", ".") + " m";
                                                    geoFeaturesLookupTable[$"{outerLoopIdx}_{innerLoopIdx}"].SelectedTrend = selectedTrend.ToString() switch
                                                    {
                                                        "1" => "decreasing",
                                                        "2" => "increasing",
                                                        "3" => "steady",
                                                        _ => "unknown"
                                                    };
                                                }
                                            }
                                            else
                                            {
                                                var tidalStationInstance = new TidalStation()
                                                {
                                                    Id = $"{outerLoopIdx}_{innerLoopIdx}",
                                                    FeatureName = new FeatureName[] { new FeatureName { DisplayName = $"{outerLoopIdx}_{innerLoopIdx}" } },
                                                    TidalHeights = new Dictionary<string, string>() { { timePoint.ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), height.ToString().Replace(",", ".") } },
                                                    TidalTrends = new Dictionary<string, string>() { { timePoint.ToString("ddMMMyyyy HHmm", new CultureInfo("en-US")), ((short)trend).ToString() } },
                                                    SelectedIndex = i,
                                                    SelectedDateTime = timePoint.ToString("ddMMMyyyy HHmm"),
                                                    SelectedHeight = Math.Round(selectedHeight, 2).ToString().Replace(",", ".") + " m",
                                                    SelectedTrend = selectedTrend.ToString() switch
                                                    {
                                                        "1" => "decreasing",
                                                        "2" => "increasing",
                                                        "3" => "steady",
                                                        _ => "unknown"
                                                    },
                                                    Geometry = geometry
                                                };

                                                geoFeaturesLookupTable.Add($"{outerLoopIdx}_{innerLoopIdx}", tidalStationInstance);
                                            }
                                        }
                                    }
                                }

                                oldTimePoint = timePoint;
                            }

                            var ratio = 50 + (int)((50.0 / (double)max) * (double)i);
                            _syncContext?.Post(new SendOrPostCallback(r =>
                            {
                                Progress?.Invoke((int)r);

                            }), ratio);
                            i++;
                        }
                    }
                }

                foreach (var geoFeature in geoFeaturesLookupTable.Values)
                {
                    geoFeatures.Add(geoFeature);
                }

                dataPackage.RawHdfData = hdf5S111Root;
                if (geoFeatures.Count > 0)
                {
                    dataPackage.GeoFeatures = geoFeatures.ToArray();
                    dataPackage.MetaFeatures = new IMetaFeature[0];
                    dataPackage.InformationFeatures = new IInformationFeature[0];
                }

            }).ConfigureAwait(false);

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
