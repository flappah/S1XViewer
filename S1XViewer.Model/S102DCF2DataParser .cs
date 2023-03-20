using S1XViewer.Model.Interfaces;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using S1XViewer.HDF;
using HDF5CSharp.DataTypes;
using S1XViewer.Model.Geometry;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Features;
using System.Globalization;
using System.Net.WebSockets;

namespace S1XViewer.Model
{
    public class S102DCF2DataParser : HdfDataParserBase, IS102DCF2DataParser
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
        public S102DCF2DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS111ProductSupport productSupport)
        {
            _datasetReader = datasetReader;
            _geometryBuilderFactory = geometryBuilderFactory;
            _productSupport = productSupport;
        }

        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            throw new NotImplementedException();
        }

        public override async Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public override async Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime)
        {
            if (string.IsNullOrEmpty(hdf5FileName))
            {
                throw new ArgumentException($"'{nameof(hdf5FileName)}' cannot be null or empty.", nameof(hdf5FileName));
            }

            var dataPackage = new S1xxDataPackage
            {
                Type = S1xxTypes.S111,
                RawXmlData = null,
                RawHdfData = null
            };

            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = await _productSupport.RetrieveHdf5FileAsync(hdf5FileName);
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

            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int)horizontalCRS);

            Hdf5Element? featureElement = hdf5S111Root.Children.Find(elm => elm.Name.Equals("/BathymetryCoverage"));
            if (featureElement == null)
            {
                return dataPackage;
            }

            double nillValueDepth = 1000000;
            double nillValueUncertainty = 1000000;
            //var featureMetaInfoElements = _datasetReader.Read<BathymetryCoverageInformationInstance>(hdf5FileName, "/Group_F/BathymetryCoverage");
            //if (featureMetaInfoElements != null)
            //{
            //    foreach (var featureMetaInfoElement in featureMetaInfoElements)
            //    {
            //        if (featureMetaInfoElement.code == "depth")
            //        {
            //            double.TryParse(featureMetaInfoElement.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out nillValueDepth);
            //        }
            //        else if (featureMetaInfoElement.code == "uncertainty")
            //        {
            //            double.TryParse(featureMetaInfoElement.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out nillValueUncertainty);
            //        }
            //    }
            //}
            
            var selectedBathymetryCoverageElement = featureElement.Children[0];
            if (selectedBathymetryCoverageElement != null)
            {
                Hdf5Element? minGroup = selectedBathymetryCoverageElement.Children[0];
                if (minGroup != null)
                {
                    var geoFeatures = new List<IGeoFeature>();

                    await Task.Run(() =>
                    {
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

                        float[,] depthsAndUncertainties =
                              _datasetReader.ReadArrayOfFloats(hdf5FileName, minGroup.Children[0].Name, numPointsLatitude, numPointsLongitude * 2);

                        if (depthsAndUncertainties == null || depthsAndUncertainties.Length == 0)
                        {
                            return;
                        }

                        for (int latIdx = 0; latIdx < numPointsLatitude; latIdx++)
                        {
                            for (int lonIdx = 0; lonIdx < numPointsLongitude; lonIdx += 2)
                            {
                                // build up featutes ard wrap 'em in datapackage
                                float depth = depthsAndUncertainties[latIdx, lonIdx];
                                float uncertainty = depthsAndUncertainties[latIdx, lonIdx + 1];

                                if (depth != nillValueDepth)
                                {
                                    double longitude = gridOriginLongitude + (((double)lonIdx / 2.0) * gridSpacingLongitudinal);
                                    double latitude = gridOriginLatitude + ((double)latIdx * gridSpacingLatitudinal);

                                    var longitudes = new double[5];
                                    var latitudes = new double[5];
                                    longitudes[0] = longitude;
                                    latitudes[0] = latitude;
                                    
                                    longitudes[1] = longitude;
                                    latitudes[1] = latitude + gridSpacingLatitudinal;
                                    
                                    longitudes[2] = longitude + gridSpacingLongitudinal;
                                    latitudes[2] = latitude + gridSpacingLatitudinal;

                                    longitudes[3] = longitude + gridSpacingLongitudinal;
                                    latitudes[3] = latitude;

                                    longitudes[4] = longitude;
                                    latitudes[4] = latitude;

                                    var geometry =
                                        _geometryBuilderFactory.Create("Polygon", longitudes, latitudes, depth, (int)horizontalCRS);

                                    var sounding = new Sounding
                                    {
                                        Id = $"Sid_{lonIdx}_{latIdx}",
                                        FeatureName = new FeatureName[] { new FeatureName { DisplayName = $"Sid_{lonIdx}_{latIdx}", } },
                                        Value = depth,
                                        Geometry = geometry
                                    };

                                    geoFeatures.Add(sounding);
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
            }

            Progress?.Invoke(100);
            return dataPackage;
        }
    }
}
