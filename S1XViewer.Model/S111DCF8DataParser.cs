using HDF5CSharp;
using HDF5CSharp.DataTypes;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Features;
using S1XViewer.Types.Interfaces;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace S1XViewer.Model
{
    public class S111DCF8DataParser : HdfDataParserBase, IS111DCF8DataParser
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
        public S111DCF8DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory)
        {
            _datasetReader = datasetReader;
            _geometryBuilderFactory = geometryBuilderFactory;
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
                return new S1xxDataPackage
                {
                    Type = S1xxTypes.Null,
                    RawXmlData = null,
                    RawHdfData = null,
                    GeoFeatures = new IGeoFeature[0],
                    MetaFeatures = new IMetaFeature[0],
                    InformationFeatures = new IInformationFeature[0]
                };
            }

            var dataPackage = new S1xxDataPackage
            {
                Type = S1xxTypes.S111,
                RawXmlData = null
            };

            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = await RetrieveHdf5FileAsync(hdf5FileName);

            // retrieve boundingbox
            var eastBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("eastBoundLongitude");
            var eastBoundLongitude = eastBoundLongitudeAttribute?.Value<double>(0.0) ?? 0.0;
            var northBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("northBoundLatitude");
            var northBoundLatitude = northBoundLatitudeAttribute?.Value<double>(0.0) ?? 0.0;
            var southBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("southBoundLatitude");
            var southBoundLatitude = southBoundLatitudeAttribute?.Value<double>(0.0) ?? 0.0;
            var westBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("westBoundLongitude");
            var westBoundLongitude = westBoundLongitudeAttribute?.Value<double>(0.0) ?? 0.0;

            var horizontalCRSAttribute = hdf5S111Root.Attributes.Find("horizontalCRS");
            var horizontalCRS = horizontalCRSAttribute?.Value<long>(4326) ?? 4326;
            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude}, (int) horizontalCRS);

            // retrieve relevant time-frame from SurfaceCurrents collection
            var selectedSurfaceFeatureElement = FindFeatureByDateTime(hdf5S111Root.Children[1].Children, selectedDateTime);
            if (selectedSurfaceFeatureElement != null)
            {
                // now retrieve positions 
                var positioningElement = selectedSurfaceFeatureElement.Children.Find(nd => nd.Name.LastPart("/") == "Positioning");
                if (positioningElement != null)
                {
                    var positionValues =
                        _datasetReader.Read<GeometryValueInstance>(hdf5FileName, positioningElement.Children[0].Name).ToArray();

                    if (positionValues == null || positionValues.Length == 0)
                    {
                        throw new Exception($"Surfacefeature with name {positioningElement.Children[0].Name} contains no positions!");
                    }

                    // retrieve directions and current speeds
                    var numberOfStationsAttribute = selectedSurfaceFeatureElement.Attributes.Find("numberOfStations");
                    var numberOfStations = numberOfStationsAttribute?.Value<long>(0) ?? 0;
                    if (numberOfStations != positionValues.Count())
                    {
                        throw new Exception("Insufficient number of position values!");
                    }

                    int stationNumber = 0;
                    var geoFeatures = new List<IGeoFeature>();
                    foreach (Hdf5Element? groupHdf5Group in selectedSurfaceFeatureElement.Children)
                    {
                        if (groupHdf5Group.Name.Contains("Group_"))
                        {
                            var startDateTimeAttribute = groupHdf5Group.Attributes.Find("startDateTime");
                            string startDateTimeString = startDateTimeAttribute?.Value<string>("") ?? "";
                            DateTime startDateTime =
                                DateTime.ParseExact(startDateTimeString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();

                            var featureNameAttribute = groupHdf5Group.Attributes.Find("stationName");
                            var featureName = featureNameAttribute?.Value<string>("") ?? "";

                            var timeIntervalAttribute = groupHdf5Group.Attributes.Find("timeRecordInterval");
                            var timeInterval = timeIntervalAttribute?.Value<long>(0) ?? 0;

                            if (timeInterval > 0)
                            {
                                var surfaceCurrentInfos =
                                    _datasetReader.Read<SurfaceCurrentInstance>(hdf5FileName, groupHdf5Group.Children[0].Name).ToArray();

                                if (surfaceCurrentInfos != null)
                                {
                                    var index = (int)((TimeSpan)(selectedDateTime - startDateTime)).TotalSeconds / timeInterval;
                                    if (index < surfaceCurrentInfos.Length)
                                    {
                                        var direction = surfaceCurrentInfos[index].direction;
                                        var speed = surfaceCurrentInfos[index].speed;

                                        var geometry =
                                            _geometryBuilderFactory.Create("Point", new double[] { positionValues[stationNumber].longitude }, new double[] { positionValues[stationNumber].latitude });
                                        var currentNonGravitationalInstance = new CurrentNonGravitational()
                                        {
                                            Id = groupHdf5Group.Name,
                                            FeatureName = new FeatureName[] { new FeatureName { DisplayName = featureName } },
                                            Orientation = new Types.ComplexTypes.Orientation { OrientationValue = direction },
                                            Speed = new Types.ComplexTypes.Speed { SpeedMaximum = speed },
                                            Geometry = geometry
                                        };
                                        geoFeatures.Add(currentNonGravitationalInstance);
                                    }
                                    else
                                    {
                                        throw new Exception($"Calulated index larger than storage of SurfaceCurrents! selectedDateTime:{selectedDateTime},startDateTime:{startDateTime},timeInterval:{timeInterval},index:{index}");
                                    }
                                }
                            }

                            stationNumber++;
                        }
                    }

                    // build up featutes ard wrap 'em in datapackage
                    if (geoFeatures.Count > 0)
                    {
                        dataPackage.RawHdfData = hdf5S111Root;
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

            // load HDF file
            //HDF5CSharp.DataTypes.Hdf5Element tree = HDF5CSharp.Hdf5.ReadTreeFileStructure(fileName);

            return new S1xxDataPackage
            {
                Type = S1xxTypes.Null,
                RawXmlData = null,
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
            return new S1xxDataPackage
            {
                Type = S1xxTypes.Null,
                RawXmlData = null,
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
            return new S1xxDataPackage
            {
                Type = S1xxTypes.Null,
                RawXmlData = null,
                RawHdfData = null,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }
    }
}
