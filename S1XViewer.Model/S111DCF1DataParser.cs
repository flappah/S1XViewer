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
    public class S111DCF1DataParser : HdfDataParserBase, IS111DCF1DataParser
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
        public S111DCF1DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS111ProductSupport productSupport, IOptionsStorage optionsStorage)
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

            string invertLatLonString = _optionsStorage.Retrieve("checkBoxInvertLatLon");
            if (!bool.TryParse(invertLatLonString, out bool invertLatLon))
            {
                invertLatLon = false;
            }
            string defaultCRSString = _optionsStorage.Retrieve("comboBoxCRS");
            _geometryBuilderFactory.InvertLatLon = invertLatLon;
            _geometryBuilderFactory.DefaultCRS = defaultCRSString;

            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = await _productSupport.RetrieveHdf5FileAsync(hdf5FileName);
            long horizontalCRS = RetrieveHorizontalCRS(hdf5S111Root, hdf5FileName);

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

            // retrieve relevant time-frame from SurfaceCurrents collection
            Hdf5Element? featureElement = hdf5S111Root.Children.Find(elm => elm.Name.Equals("/SurfaceCurrent"));
            if (featureElement == null)
            {
                return dataPackage;
            }

            var selectedSurfaceFeatureElement = _productSupport.FindFeatureByDateTime(featureElement.Children, selectedDateTime);
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

                    var timeIntervalAttribute = selectedSurfaceFeatureElement.Attributes.Find("timeRecordInterval");
                    var timeInterval = timeIntervalAttribute?.Value<long>(0) ?? 0;

                    if (timeInterval > 0)
                    {
                        IGeoFeature[] geoFeatures = new IGeoFeature[numberOfStations];

                        var startDateTimeAttribute = selectedSurfaceFeatureElement.Attributes.Find("dateTimeOfFirstRecord");
                        string startDateTimeString = startDateTimeAttribute?.Value<string>("") ?? "";
                        DateTime startDateTime =
                            DateTime.ParseExact(startDateTimeString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();

                        int stationNumber = 0;

                        foreach (Hdf5Element? groupHdf5Group in selectedSurfaceFeatureElement.Children)
                        {
                            if (groupHdf5Group.Name.Contains("Group_"))
                            {
                                var surfaceCurrentInfos =
                                    _datasetReader.Read<SurfaceCurrentInstance>(hdf5FileName, groupHdf5Group.Children[0].Name).ToArray();

                                if (surfaceCurrentInfos != null)
                                {
                                    // build up features ard wrap 'em in datapackage
                                    var index = (int)((TimeSpan)(selectedDateTime - startDateTime)).TotalSeconds / timeInterval;
                                    /* 
                                     * with varying groupsizes indexes can sometimes fall outside the boundary because different 
                                     * reference tidalstations have timeseries of differents length wrapped inside 1 package. 
                                     * Set to limits if necessary
                                     */
                                    if (index < 0)
                                    {
                                        index = 0;
                                    }
                                    else if (index >= surfaceCurrentInfos.Length)
                                    {
                                        index = surfaceCurrentInfos.Length - 1;
                                    }

                                    var direction = surfaceCurrentInfos[index].direction;
                                    var speed = surfaceCurrentInfos[index].speed;

                                    Esri.ArcGISRuntime.Geometry.Geometry geometry =
                                        _geometryBuilderFactory.Create("Point", new double[] { positionValues[stationNumber].longitude }, new double[] { positionValues[stationNumber].latitude }, (int)horizontalCRS);

                                    var currentNonGravitationalInstance = new CurrentNonGravitational()
                                    {
                                        Id = groupHdf5Group.Name,
                                        FeatureName = new FeatureName[] { new FeatureName { DisplayName = groupHdf5Group.Name } },
                                        Orientation = new Types.ComplexTypes.Orientation { OrientationValue = direction },
                                        Speed = new Types.ComplexTypes.Speed { SpeedMaximum = speed },
                                        Geometry = geometry
                                    };
                                    geoFeatures[stationNumber] = currentNonGravitationalInstance;
                                }

                                stationNumber++;
                            }
                        }

                        dataPackage.RawHdfData = hdf5S111Root;
                        if (geoFeatures.Length > 0)
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
