﻿using HDF5CSharp.DataTypes;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Features;
using S1XViewer.Types.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Model
{
    public class S111DCF8DataParser : DataParserBase, IS111DCF8DataParser
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
        /// 
        /// </summary>
        /// <param name="surfaceFeatures"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        private Hdf5Element? FindRelevantSurfaceFeatureByDateTime(List<Hdf5Element> surfaceFeatures, DateTime? selectedDateTime)
        {
            if (surfaceFeatures is null)
            {
                throw new ArgumentNullException(nameof(surfaceFeatures));
            }

            if (selectedDateTime is null)
            {
                throw new ArgumentNullException(nameof(selectedDateTime));
            }

            foreach (Hdf5Element? hdf5Element in surfaceFeatures)
            {
                if (hdf5Element != null)
                {
                    var dateTimeOfFirstRecordAttribute = hdf5Element.Attributes.Find("dateTimeOfFirstRecord");
                    var dateTimeOfLastRecordAttribute = hdf5Element.Attributes.Find("dateTimeOfLastRecord");

                    if (dateTimeOfFirstRecordAttribute != null && dateTimeOfLastRecordAttribute != null)
                    {
                        if (dateTimeOfFirstRecordAttribute.Values != null && dateTimeOfLastRecordAttribute.Values != null)
                        {
                            if (((string[])dateTimeOfFirstRecordAttribute.Values).Length > 0 &&
                                ((string[])dateTimeOfLastRecordAttribute.Values).Length > 0)
                            {
                                DateTime dateTimeOfFirstRecord =
                                    DateTime.ParseExact(((string[])dateTimeOfFirstRecordAttribute.Values)[0], "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();

                                DateTime dateTimeOfLastRecord =
                                    DateTime.ParseExact(((string[])dateTimeOfLastRecordAttribute.Values)[0], "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();

                                if (selectedDateTime > dateTimeOfFirstRecord &&
                                    selectedDateTime < dateTimeOfLastRecord)
                                {
                                    return hdf5Element;
                                }
                            }
                        }
                    }
                }
            }

            return null;
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
                    GeoFeatures = new IGeoFeature[0],
                    MetaFeatures = new IMetaFeature[0],
                    InformationFeatures = new IInformationFeature[0]
                };
            }

            Progress?.Invoke(50);

            var hdf5S111Tree = await Task.Factory.StartNew((name) =>
            {
                // load HDF file, spawned in a seperate task to keep UI responsive!
                return HDF5CSharp.Hdf5.ReadTreeFileStructure(name.ToString());

            }, hdf5FileName).ConfigureAwait(false);

            // retrieve relevant time-frame from SurfaceCurrents collection
            var selectedSurfaceFeature = FindRelevantSurfaceFeatureByDateTime(hdf5S111Tree.Children[1].Children, selectedDateTime);
            if (selectedSurfaceFeature != null)
            {
                // now retrieve positions 
                var positioningElement = selectedSurfaceFeature.Children.Find(nd => nd.Name.LastPart("/") == "Positioning");
                if (positioningElement != null)
                {
                    var positionValues =
                        _datasetReader.Read<GeometryValueInstance>(hdf5FileName, positioningElement.Children[0].Name).ToArray();

                    if (positionValues != null)
                    {
                        // retrieve directions and current speeds
                        var numberOfStationsAttribute = selectedSurfaceFeature.Attributes.Find("numberOfStations");
                        var numberOfStations =
                            numberOfStationsAttribute != null &&
                            numberOfStationsAttribute.Values != null &&
                            ((long[])numberOfStationsAttribute.Values).Length > 0 ? ((long[])numberOfStationsAttribute.Values)[0] : 0;

                        if (numberOfStations != positionValues.Count())
                        {
                            throw new Exception("Insufficient number of position values!");
                        }

                        int stationNumber = 0;
                        var geoFeatures = new List<IGeoFeature>();
                        foreach (Hdf5Element? groupHdf5Group in selectedSurfaceFeature.Children)
                        {
                            if (groupHdf5Group.Name.Contains("Group_"))
                            {
                                var startDateTimeAttribute = groupHdf5Group.Attributes.Find("startDateTime");
                                DateTime startDateTime;
                                if (startDateTimeAttribute != null &&
                                    startDateTimeAttribute.Values != null &&
                                    ((string[])startDateTimeAttribute.Values).Length > 0)
                                {
                                    startDateTime =
                                        DateTime.ParseExact(((string[])startDateTimeAttribute.Values)[0], "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();

                                    var timeIntervalAttribute = groupHdf5Group.Attributes.Find("timeRecordInterval");
                                    var timeInterval =
                                        timeIntervalAttribute != null &&
                                        timeIntervalAttribute.Values != null &&
                                        ((long[])timeIntervalAttribute.Values).Length > 0 ? ((long[])timeIntervalAttribute.Values)[0] : 0;

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
                                }

                                stationNumber++;
                            }
                        }

                        // build up featutes and wrap 'em in datapackage

                    }
                }
            }

            Progress?.Invoke(100);

            return new S1xxDataPackage
            {
                Type = S1xxTypes.Null,
                RawXmlData = null,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
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
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }
    }
}
