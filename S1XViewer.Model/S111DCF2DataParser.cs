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
using Windows.Media.Capture;

namespace S1XViewer.Model
{
    public class S111DCF2DataParser : HdfDataParserBase, IS111DCF2DataParser
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
        public S111DCF2DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS111ProductSupport productSupport, IOptionsStorage optionsStorage)
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
                Type = S1xxTypes.S111,
                RawHdfData = null
            };

            string invertLonLatString = _optionsStorage.Retrieve("checkBoxInvertLonLat");
            if (!bool.TryParse(invertLonLatString, out bool invertLonLat))
            {
                invertLonLat = false;
                dataPackage.InvertLonLat = invertLonLat;
            }
            string defaultCRSString = _optionsStorage.Retrieve("comboBoxCRS");
            _geometryBuilderFactory.InvertLonLat = invertLonLat;
            _geometryBuilderFactory.DefaultCRS = defaultCRSString;

            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = await _productSupport.RetrieveHdf5FileAsync(hdf5FileName);
            long horizontalCRS = RetrieveHorizontalCRS(hdf5S111Root, hdf5FileName);

            // retrieve relevant time-frame from SurfaceCurrents collection
            Hdf5Element? featureElement = hdf5S111Root.Children.Find(elm => elm.Name.Equals("/SurfaceCurrent"));
            if (featureElement == null)
            {
                return dataPackage;
            }

            // check position ordering lon/lat vs lat/lon
            var axisNameElement = featureElement.Children.Find(elm => elm.Name.Equals("/SurfaceCurrent/axisNames"));
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
            var eastBoundLongitude = eastBoundLongitudeAttribute?.Value<double>(0f) ?? 0.0;
            var northBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("northBoundLatitude");
            var northBoundLatitude = northBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var southBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("southBoundLatitude");
            var southBoundLatitude = southBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var westBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("westBoundLongitude");
            var westBoundLongitude = westBoundLongitudeAttribute?.Value<double>(0f) ?? 0f;

            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int) horizontalCRS);

            Hdf5Element? groupFElement = hdf5S111Root.Children.Find(elm => elm.Name.Equals("/Group_F"));
            if (groupFElement == null)
            {
                return dataPackage;
            }

            // retrieve values for undefined datacells
            double nillValueSpeed = -9999.0;
            double nillValueDirection = -9999.0;
            var featureMetaInfoElements = _datasetReader.ReadCompound<SurfaceCurrentInformationInstance>(hdf5FileName, "/Group_F/SurfaceCurrent");
            if (featureMetaInfoElements.values.Length > 0)
            {
                foreach (var featureMetainfoElementValue in  featureMetaInfoElements.values) 
                {
                    if (featureMetainfoElementValue.code.Equals("surfaceCurrentSpeed"))
                    {
                        if (float.TryParse(featureMetainfoElementValue.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out float speedFillValue))
                        {
                            nillValueSpeed = speedFillValue;
                        }
                    }
                    else if (featureMetainfoElementValue.code.Equals("surfaceCurrentDirection") || featureMetainfoElementValue.code.Equals("sufaceCurrentDirection"))
                    {
                        if (float.TryParse(featureMetainfoElementValue.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out float speedDirectionValue))
                        {
                            nillValueDirection += speedDirectionValue;
                        }
                    }
                }
            }

            // retrieve relevant data specified by datetime
            Hdf5Element? minGroup = _productSupport.FindGroupByDateTime(featureElement.Children, selectedDateTime);
            if (minGroup != null)
            {
                var geoFeatures = new List<IGeoFeature>();
                await Task.Run(() =>
                {
                    //we've found the relevant group. Use this group to create features on by calculating its position
                    var minGroupParentNode = (Hdf5Element)minGroup.Parent;
                    var gridOriginLatitudeElement = minGroupParentNode.Attributes.Find("gridOriginLatitude");
                    var gridOriginLatitude = gridOriginLatitudeElement?.Value<double>() ?? -999.0;
                    var gridOriginLongitudeElement = minGroupParentNode.Attributes.Find("gridOriginLongitude");
                    var gridOriginLongitude = gridOriginLongitudeElement?.Value<double>() ?? -999.0;
                    var gridSpacingLatitudinalElement = minGroupParentNode.Attributes.Find("gridSpacingLatitudinal");
                    var gridSpacingLatitudinal = gridSpacingLatitudinalElement?.Value<double>() ?? -999.0;
                    var gridSpacingLongitudinalElement = minGroupParentNode.Attributes.Find("gridSpacingLongitudinal");
                    var gridSpacingLongitudinal = gridSpacingLongitudinalElement?.Value<double>() ?? -999.0;

                    if (gridOriginLatitude == -999.0 || gridOriginLongitude == -999.0 || gridSpacingLatitudinal == -999.0 || gridSpacingLongitudinal == -999.0)
                    {
                        return;
                    }

                    var numPointsLatitudinalElement = minGroupParentNode.Attributes.Find("numPointsLatitudinal");
                    int numPointsLatitude = numPointsLatitudinalElement?.Value<int>() ?? -1;
                    var numPointsLongitudinalNode = minGroupParentNode.Attributes.Find("numPointsLongitudinal");
                    int numPointsLongitude = numPointsLongitudinalNode?.Value<int>() ?? -1;

                    if (numPointsLatitude == -1 || numPointsLongitude == -1)
                    {
                        return;
                    }

                    var currentDataset =
                          _datasetReader.ReadArrayOfFloats(hdf5FileName, minGroup.Children[0].Name, numPointsLatitude, numPointsLongitude * 2);

                    if (currentDataset.members.Length == 0)
                    {
                        return;
                    }

                    for (int latIdx = 0; latIdx < numPointsLatitude; latIdx++)
                    {
                        for (int lonIdx = 0; lonIdx < (numPointsLongitude * 2); lonIdx += 2)
                        {
                            // build up featutes ard wrap 'em in datapackage
                            float speed;
                            float direction;
                            if (currentDataset.members[0].Equals("surfaceCurrentDirection"))
                            {
                                direction = currentDataset.values[latIdx, lonIdx];
                                speed = currentDataset.values[latIdx, lonIdx + 1];
                            }
                            else
                            {
                                speed = currentDataset.values[latIdx, lonIdx];
                                direction = currentDataset.values[latIdx, latIdx + 1];
                            }

                            if (speed != nillValueSpeed && direction != nillValueDirection)
                            {
                                double longitude = gridOriginLongitude + (((double)lonIdx / 2.0) * gridSpacingLongitudinal);
                                double latitude = gridOriginLatitude + ((double)latIdx * gridSpacingLatitudinal);

                                var geometry =
                                    _geometryBuilderFactory.Create("Point", new double[] { longitude }, new double[] { latitude }, (int)horizontalCRS);

                                var currentNonGravitationalInstance = new CurrentNonGravitational()
                                {
                                    Id = minGroup.Name + $"_{latIdx}_{lonIdx}",
                                    FeatureName = new FeatureName[] { new FeatureName { DisplayName = $"VS_{longitude.ToString().Replace(",", ".")}_{latitude.ToString().Replace(",", ".")}" } },
                                    Orientation = new Orientation { OrientationValue = direction },
                                    Speed = new Speed { SpeedMaximum = speed },
                                    Geometry = geometry
                                };

                                geoFeatures.Add(currentNonGravitationalInstance);
                            }
                        }

                        Progress?.Invoke(50 + (int)((50.0 / (double)numPointsLatitude) * (double)latIdx));
                    }

                    dataPackage.RawHdfData = hdf5S111Root;
                    if (geoFeatures.Count > 0)
                    {
                        dataPackage.GeoFeatures = geoFeatures.ToArray();
                        dataPackage.MetaFeatures = new IMetaFeature[0];
                        dataPackage.InformationFeatures = new IInformationFeature[0];
                    }

                }).ConfigureAwait(false);
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
                Type = S1xxTypes.S111,
                RawHdfData = null
            };

            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = _productSupport.RetrieveHdf5FileAsync(hdf5FileName).GetAwaiter().GetResult();
            long horizontalCRS = RetrieveHorizontalCRS(hdf5S111Root, hdf5FileName);

            // retrieve boundingbox
            var eastBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("eastBoundLongitude");
            var eastBoundLongitude = eastBoundLongitudeAttribute?.Value<double>(0f) ?? 0.0;
            var northBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("northBoundLatitude");
            var northBoundLatitude = northBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var southBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("southBoundLatitude");
            var southBoundLatitude = southBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var westBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("westBoundLongitude");
            var westBoundLongitude = westBoundLongitudeAttribute?.Value<double>(0f) ?? 0f;

            string invertLatLonString = _optionsStorage.Retrieve("checkBoxInvertLonLat");
            if (!bool.TryParse(invertLatLonString, out bool invertLatLon))
            {
                invertLatLon = false;
            }
            string defaultCRSString = _optionsStorage.Retrieve("comboBoxCRS");
            _geometryBuilderFactory.InvertLonLat = invertLatLon;
            _geometryBuilderFactory.DefaultCRS = defaultCRSString;

            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int)horizontalCRS);

            Hdf5Element? featureElement = hdf5S111Root.Children.Find(elm => elm.Name.Equals("/SurfaceCurrent"));
            if (featureElement == null)
            {
                return dataPackage;
            }

            double nillValueSpeed = -9999.0;
            double nillValueDirection = -9999.0;
            var featureMetaInfoElements = _datasetReader.ReadCompound<SurfaceCurrentInformationInstance>(hdf5FileName, "/Group_F/SurfaceCurrent");
            if (featureMetaInfoElements.members.Length > 0)
            {
                foreach (var featureMetainfoElementValue in featureMetaInfoElements.values)
                {
                    if (featureMetainfoElementValue.code.Equals("surfaceCurrentSpeed"))
                    {
                        if (float.TryParse(featureMetainfoElementValue.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out float speedFillValue))
                        {
                            nillValueSpeed = speedFillValue;
                        }
                    }
                    else if (featureMetainfoElementValue.code.Equals("surfaceCurrentDirection") || featureMetainfoElementValue.code.Equals("sufaceCurrentDirection"))
                    {
                        if (float.TryParse(featureMetainfoElementValue.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out float speedDirectionValue))
                        {
                            nillValueDirection += speedDirectionValue;
                        }
                    }
                }
            }

            Hdf5Element? minGroup = _productSupport.FindGroupByDateTime(featureElement.Children, selectedDateTime);
            if (minGroup != null)
            {
                var geoFeatures = new List<IGeoFeature>();

                //we've found the relevant group. Use this group to create features on by calculating its position
                var minGroupParentNode = (Hdf5Element)minGroup.Parent;
                var gridOriginLatitudeElement = minGroupParentNode.Attributes.Find("gridOriginLatitude");
                var gridOriginLatitude = gridOriginLatitudeElement?.Value<double>() ?? -1.0;
                var gridOriginLongitudeElement = minGroupParentNode.Attributes.Find("gridOriginLongitude");
                var gridOriginLongitude = gridOriginLongitudeElement?.Value<double>() ?? -1.0;
                var gridSpacingLatitudinalElement = minGroupParentNode.Attributes.Find("gridSpacingLatitudinal");
                var gridSpacingLatitudinal = gridSpacingLatitudinalElement?.Value<double>() ?? -1.0;
                var gridSpacingLongitudinalElement = minGroupParentNode.Attributes.Find("gridSpacingLongitudinal");
                var gridSpacingLongitudinal = gridSpacingLongitudinalElement?.Value<double>() ?? -1.0;

                if (gridOriginLatitude == -1.0 || gridOriginLongitude == -1.0 || gridSpacingLatitudinal == -1.0 || gridSpacingLongitudinal == -1.0)
                {
                    return dataPackage;
                }

                var numPointsLatitudinalElement = minGroupParentNode.Attributes.Find("numPointsLatitudinal");
                int numPointsLatitude = numPointsLatitudinalElement?.Value<int>() ?? -1;
                var numPointsLongitudinalNode = minGroupParentNode.Attributes.Find("numPointsLongitudinal");
                int numPointsLongitude = numPointsLongitudinalNode?.Value<int>() ?? -1;

                if (numPointsLatitude == -1 || numPointsLongitude == -1)
                {
                    return dataPackage;
                }

                var currentDataset =
                      _datasetReader.ReadArrayOfFloats(hdf5FileName, minGroup.Children[0].Name, numPointsLatitude, numPointsLongitude * 2);

                if (currentDataset.members.Length == 0)
                {
                    return dataPackage;
                }

                for (int latIdx = 0; latIdx < numPointsLatitude; latIdx++)
                {
                    for (int lonIdx = 0; lonIdx < numPointsLongitude; lonIdx += 2)
                    {
                        // build up featutes ard wrap 'em in datapackage
                        float speed;
                        float direction;
                        if (currentDataset.members[0].Equals("surfaceCurrentDirection"))
                        {
                            direction = currentDataset.values[latIdx, lonIdx];
                            speed = currentDataset.values[latIdx, lonIdx + 1];
                        }
                        else
                        {
                            speed = currentDataset.values[latIdx, lonIdx];
                            direction = currentDataset.values[latIdx, latIdx + 1];
                        }

                        if (speed != nillValueSpeed && direction != nillValueDirection)
                        {
                            double longitude = gridOriginLongitude + (((double)lonIdx / 2.0) * gridSpacingLongitudinal);
                            double latitude = gridOriginLatitude + ((double)latIdx * gridSpacingLatitudinal);

                            var geometry =
                                _geometryBuilderFactory.Create("Point", new double[] { longitude }, new double[] { latitude }, (int)horizontalCRS);

                            var currentNonGravitationalInstance = new CurrentNonGravitational()
                            {
                                Id = minGroup.Name + $"_{latIdx}_{lonIdx}",
                                FeatureName = new FeatureName[] { new FeatureName { DisplayName = $"VS_{longitude.ToString().Replace(",", ".")}_{latitude.ToString().Replace(",", ".")}" } },
                                Orientation = new Orientation { OrientationValue = direction },
                                Speed = new Speed { SpeedMaximum = speed },
                                Geometry = geometry
                            };

                            geoFeatures.Add(currentNonGravitationalInstance);
                        }
                    }

                    Progress?.Invoke(50 + (int)((50.0 / (double)numPointsLatitude) * (double)latIdx));
                }

                if (geoFeatures.Count > 0)
                {
                    dataPackage.RawHdfData = hdf5S111Root;
                    dataPackage.GeoFeatures = geoFeatures.ToArray();
                    dataPackage.MetaFeatures = new IMetaFeature[0];
                    dataPackage.InformationFeatures = new IInformationFeature[0];
                }
            }

            Progress?.Invoke(100);
            return dataPackage;
        }

        /// <summary>
        ///     No implementation!
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
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

        /// <summary>
        ///     No implementation!
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
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
    }
}
