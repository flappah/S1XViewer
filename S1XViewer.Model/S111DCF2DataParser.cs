using HDF5CSharp.DataTypes;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Features;
using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace S1XViewer.Model
{
    public class S111DCF2DataParser : HdfDataParserBase, IS111DCF2DataParser
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
        public S111DCF2DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory)
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
                RawXmlData = null,
                RawHdfData = null
            };

            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = await RetrieveHdf5FileAsync(hdf5FileName);

            // retrieve boundingbox
            var eastBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("eastBoundLongitude");
            var eastBoundLongitude = eastBoundLongitudeAttribute?.Value<float>(0f) ?? 0.0;
            var northBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("northBoundLatitude");
            var northBoundLatitude = northBoundLatitudeAttribute?.Value<float>(0f) ?? 0f;
            var southBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("southBoundLatitude");
            var southBoundLatitude = southBoundLatitudeAttribute?.Value<float>(0f) ?? 0f;
            var westBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("westBoundLongitude");
            var westBoundLongitude = westBoundLongitudeAttribute?.Value<float>(0f) ?? 0f;

            var horizontalCRSAttribute = hdf5S111Root.Attributes.Find("horizontalCRS");
            var horizontalCRS = horizontalCRSAttribute?.Value<long>(4326) ?? 4326;
            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int)horizontalCRS);

            Hdf5Element? minGroup = FindGroupByDateTime(hdf5S111Root.Children[1].Children, selectedDateTime);
            if (minGroup != null)
            {
                await Task.Run(() =>
                {
                    //we've found the relevant group. Use this group to create features on by calculating its position
                    var minGroupParentNode = (Hdf5Element)minGroup.Parent;
                    var gridOriginLatitudeElement = minGroupParentNode.Attributes.Find("gridOriginLatitude");
                    var gridOriginLatitude = gridOriginLatitudeElement?.Value<float>();
                    var gridOriginLongitudeElement = minGroupParentNode.Attributes.Find("gridOriginLongitude");
                    var gridOriginLongitude = gridOriginLongitudeElement?.Value<float>();
                    var gridSpacingLatitudinalElement = minGroupParentNode.Attributes.Find("gridSpacingLatitudinal");
                    var gridSpacingLatitudinal = gridSpacingLatitudinalElement?.Value<float>();
                    var gridSpacingLongitudinalElement = minGroupParentNode.Attributes.Find("gridSpacingLongitudinal");
                    var gridSpacingLongitudinal = gridSpacingLongitudinalElement?.Value<float>();

                    var numPointsLatitudinalElement = minGroupParentNode.Attributes.Find("numPointsLatitudinal");
                    int numPointsLatitude = numPointsLatitudinalElement?.Value<int>() ?? -1;

                    var numPointsLongitudinalNode = minGroupParentNode.Attributes.Find("numPointsLongitudinal");
                    int numPointsLongitude = numPointsLongitudinalNode?.Value<int>() ?? -1;

                    if (numPointsLatitude != -1 && numPointsLongitude != -1)
                    {
                        float[,] speedAndDirectionValues =
                              _datasetReader.ReadArrayOfFloats(hdf5FileName, minGroup.Children[0].Name, numPointsLatitude, numPointsLongitude);

                        var geoFeatures = new List<IGeoFeature>();
                        for (int latIdx = 0; latIdx < numPointsLatitude; latIdx++)
                        {
                            for (int lonIdx = 0; lonIdx < numPointsLongitude; lonIdx += 2)
                            {
                                float speed = speedAndDirectionValues[latIdx, lonIdx];
                                float direction = speedAndDirectionValues[latIdx, lonIdx + 1];

                                if (speed != -9999.0 && direction != -9999.0)
                                {
                                    double longitude = gridOriginLongitude + ((lonIdx / 2) * gridSpacingLongitudinal) ?? -1.0;
                                    double latitude = gridOriginLatitude + (latIdx * gridSpacingLatitudinal) ?? -1.0;

                                    if (longitude != -1.0 && latitude != -1.0)
                                    {
                                        var geometry =
                                            _geometryBuilderFactory.Create("Point", new double[] { longitude }, new double[] { latitude });
                                        var currentNonGravitationalInstance = new CurrentNonGravitational()
                                        {
                                            Id = minGroup.Name,
                                            FeatureName = new FeatureName[] { new FeatureName { DisplayName = $"VS_{longitude.ToString(new CultureInfo("en-US"))}_{latitude.ToString(new CultureInfo("en-US"))}" } },
                                            Orientation = new Types.ComplexTypes.Orientation { OrientationValue = direction },
                                            Speed = new Types.ComplexTypes.Speed { SpeedMaximum = speed },
                                            Geometry = geometry
                                        };
                                        geoFeatures.Add(currentNonGravitationalInstance);
                                    }
                                }
                            }

                            Progress?.Invoke(50 + ((50 / numPointsLatitude) * latIdx));
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
