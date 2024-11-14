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
using System;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Model
{
    public class S111DCF4V20DataParser : HdfDataParserBase, IS111DCF4V20DataParser
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
        public S111DCF4V20DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS111ProductSupport productSupport, IOptionsStorage optionsStorage)
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

            var dataPackage = new S111DataPackage
            {
                FileName = hdf5FileName,
                Type = S1xxTypes.S111,
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
            Hdf5Element? featureElement = hdf5S111Root.Children.Find(elm => elm.Name.Equals("/SurfaceCurrent"));
            if (featureElement == null)
            {
                return dataPackage;
            }

            // check position ordering lon/lat vs lat/lon
            var axisNameElement = featureElement.Children.Find(elm => elm.Name.Equals("/SurfaceCurrent/axisNames"));
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
            var eastBoundLongitude = eastBoundLongitudeAttribute?.Value<double>(0.0) ?? 0.0;
            var northBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("northBoundLatitude");
            var northBoundLatitude = northBoundLatitudeAttribute?.Value<double>(0.0) ?? 0.0;
            var southBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("southBoundLatitude");
            var southBoundLatitude = southBoundLatitudeAttribute?.Value<double>(0.0) ?? 0.0;
            var westBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("westBoundLongitude");
            var westBoundLongitude = westBoundLongitudeAttribute?.Value<double>(0.0) ?? 0.0;

            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int)horizontalCRS);

            var selectedSurfaceFeatureElement = featureElement.Children[0];
            if (selectedSurfaceFeatureElement != null)
            {
                // now retrieve positions 
                var positioningElement = selectedSurfaceFeatureElement.Children.Find(nd => nd.Name.LastPart("/") == "Positioning");
                if (positioningElement != null)
                {
                    GeometryValueInstance[] positionValues =
                        _datasetReader.Read<GeometryValueInstance>(hdf5FileName, positioningElement.Children[0].Name).ToArray();

                    if (positionValues == null || positionValues.Length == 0)
                    {
                        throw new Exception($"Surfacefeature with name {positioningElement.Children[0].Name} contains no positions!");
                    }

                    if (String.IsNullOrEmpty(utmZone) == false)
                    {
                        for (int i = 0; i < positionValues.Length; i++)
                        {
                            GeoAPI.Geometries.Coordinate transformedCoordinate =
                                TransformUTMToWGS84(new GeoAPI.Geometries.Coordinate(positionValues[i].longitude, positionValues[i].latitude), utmZone);

                            positionValues[i].longitude = transformedCoordinate.X;
                            positionValues[i].latitude = transformedCoordinate.Y;
                        }
                    }

                    // retrieve directions and current speeds
                    var numberOfStationsAttribute = selectedSurfaceFeatureElement.Attributes.Find("numberOfStations");
                    var numberOfStations = numberOfStationsAttribute?.Value<long>(0) ?? 0;

                    var timeIntervalAttribute = selectedSurfaceFeatureElement.Attributes.Find("timeRecordInterval");
                    var timeInterval = timeIntervalAttribute?.Value<long>(0) ?? 0;

                    if (timeInterval > 0)
                    {
                        var geoFeatures = new List<IGeoFeature>();

                        await Task.Run(() =>
                        {
                            var startDateTimeAttribute = selectedSurfaceFeatureElement.Attributes.Find("dateTimeOfFirstRecord");
                            string startDateTimeString = startDateTimeAttribute?.Value<string>("") ?? "";
                            DateTime startDateTime =
                                DateTime.ParseExact(startDateTimeString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();

                            int stationNumber = 0;
                            int maxCount = selectedSurfaceFeatureElement.Children.Count;

                            foreach (Hdf5Element? groupHdf5Group in selectedSurfaceFeatureElement.Children)
                            {
                                if (groupHdf5Group.Name.Contains("Group_"))
                                {
                                    var surfaceCurrentInfos =
                                        _datasetReader.Read<SurfaceCurrentInstance>(hdf5FileName, groupHdf5Group.Children[0].Name).ToArray();

                                    if (surfaceCurrentInfos != null)
                                    {
                                        int i = 0;
                                        foreach (SurfaceCurrentInstance surfaceCurrentInfo in surfaceCurrentInfos)
                                        {
                                            var direction = surfaceCurrentInfo.direction;
                                            var speed = surfaceCurrentInfo.speed;

                                            var geometry =
                                                _geometryBuilderFactory.Create("Point", new double[] { positionValues[i].longitude }, new double[] { positionValues[i].latitude }, (int)horizontalCRS);

                                            var timestamp = startDateTime.AddSeconds(timeInterval * i);

                                            var currentNonGravitationalInstance = new CurrentNonGravitational()
                                            {
                                                Id = timestamp.ToString("yyyyMMddTHHmmss"),
                                                FeatureName = new FeatureName[] { new FeatureName { DisplayName = timestamp.ToString("yyyy-MM-ddTHH:mm:ss") } },
                                                Orientation = new Types.ComplexTypes.Orientation { OrientationValue = direction },
                                                Speed = new Types.ComplexTypes.Speed { SpeedMaximum = speed },
                                                Geometry = geometry
                                            };

                                            geoFeatures.Add(currentNonGravitationalInstance);
                                            i++;
                                        }
                                    }

                                    stationNumber++;
                                }

                                var ratio = 50 + (int)((50.0 / (double)maxCount) * (double)stationNumber);
                                _syncContext?.Post(new SendOrPostCallback(r =>
                                {
                                    Progress?.Invoke((int)r);

                                }), ratio);
                            }
                        }).ConfigureAwait(false);

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
        /// <exception cref="NotImplementedException"></exception>
        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {

            // load HDF file
            //HDF5CSharp.DataTypes.Hdf5Element tree = HDF5CSharp.Hdf5.ReadTreeFileStructure(fileName);

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
