﻿using HDF5CSharp.DataTypes;
using Microsoft.Isam.Esent.Interop;
using OSGeo.GDAL;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Features;
using S1XViewer.Types.Interfaces;
using System.Data.Common;
using System.Xml;

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
        public S102DCF2DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS102ProductSupport productSupport)
        {
            _datasetReader = datasetReader;
            _geometryBuilderFactory = geometryBuilderFactory;
            _productSupport = productSupport;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataPackage"></param>
        private void CreateTiff(S102DataPackage dataPackage)
        {
            var tempPath = Path.GetTempPath();            
            GdalConfiguration.ConfigureGdal();

            string tiffFileName = $"{tempPath}{dataPackage.FileName.LastPart(@"\")}.tif";

            try
            {
                if (File.Exists(tiffFileName) == true)
                {
                    File.Delete(tiffFileName);
                }

                using (Dataset outImage = Gdal.GetDriverByName("GTiff").Create(tiffFileName, dataPackage.numPointsX, dataPackage.numPointsY, 1, DataType.GDT_Float32, null))
                {
                    Band outBand = outImage.GetRasterBand(1);
                    var outData = new float[dataPackage.numPointsY * dataPackage.numPointsX];
                    int i = 0;
                    for (int yIdx = dataPackage.numPointsY - 1; yIdx >= 0; yIdx--)
                    {
                        for (int xIdx = 0; xIdx < dataPackage.numPointsX; xIdx++)
                        {
                            outData[i++] = dataPackage.Data[yIdx, xIdx];
                        }
                    }

                    outBand.WriteRaster(0, 0, dataPackage.numPointsX, dataPackage.numPointsY, outData, dataPackage.numPointsX, dataPackage.numPointsY, 0, 0);
                    outBand.FlushCache();
                    outBand.Dispose();
                    outImage.FlushCache();
                    outImage.Dispose();
                }
            }
            catch (Exception) { }
            finally
            {
                dataPackage.TiffFileName = tiffFileName;
            }
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

            var dataPackage = new S102DataPackage
            {
                FileName = hdf5FileName,
                Type = S1xxTypes.S102,
                RawHdfData = null
            };

            Progress?.Invoke(50);

            Hdf5Element hdf5S102Root = await _productSupport.RetrieveHdf5FileAsync(hdf5FileName);
            long horizontalCRS = RetrieveHorizontalCRS(hdf5S102Root, hdf5FileName);

            // retrieve boundingbox
            var eastBoundLongitudeAttribute = hdf5S102Root.Attributes.Find("eastBoundLongitude");
            var eastBoundLongitude = eastBoundLongitudeAttribute?.Value<double>(0f) ?? 0.0;
            var northBoundLatitudeAttribute = hdf5S102Root.Attributes.Find("northBoundLatitude");
            var northBoundLatitude = northBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var southBoundLatitudeAttribute = hdf5S102Root.Attributes.Find("southBoundLatitude");
            var southBoundLatitude = southBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var westBoundLongitudeAttribute = hdf5S102Root.Attributes.Find("westBoundLongitude");
            var westBoundLongitude = westBoundLongitudeAttribute?.Value<double>(0f) ?? 0f;

            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int)horizontalCRS);

            Hdf5Element? featureElement = hdf5S102Root.Children.Find(elm => elm.Name.Equals("/BathymetryCoverage"));
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

                        float[,] depthsAndUncertainties =
                              _datasetReader.ReadArrayOfFloats(hdf5FileName, minGroup.Children[0].Name, numPointsLatitude, numPointsLongitude * 2);

                        if (depthsAndUncertainties == null || depthsAndUncertainties.Length == 0)
                        {
                            return;
                        }

                        dataPackage.minX = gridOriginLongitude;
                        dataPackage.minY = gridOriginLatitude;
                        dataPackage.maxX = eastBoundLongitude;
                        dataPackage.maxY = northBoundLatitude;
                        dataPackage.dY = gridSpacingLongitudinal;
                        dataPackage.dX = gridSpacingLatitudinal;
                        dataPackage.noDataValue = nillValueDepth;
                        dataPackage.numPointsX = numPointsLongitude;
                        dataPackage.numPointsY = numPointsLatitude;

                        dataPackage.Data = new float[numPointsLatitude, numPointsLongitude];
                        for (int yIdx = 0; yIdx < numPointsLatitude; yIdx++)
                        {
                            for (int xIdx = 0; xIdx < (numPointsLongitude * 2); xIdx += 2)
                            {
                                dataPackage.Data[yIdx, (int)xIdx / 2] = depthsAndUncertainties[yIdx, xIdx];
                            }
                        }

                        CreateTiff(dataPackage);

                        for (int yIdx = 0; yIdx < numPointsLatitude; yIdx++)
                        {
                            for (int xIdx = 0; xIdx < (numPointsLongitude * 2); xIdx += 2)
                            {
                                // build up featutes ard wrap 'em in datapackage
                                float depth = depthsAndUncertainties[yIdx, xIdx];
                                float uncertainty = depthsAndUncertainties[yIdx, xIdx + 1];

                                if (depth != nillValueDepth)
                                {
                                    double longitude = gridOriginLongitude + (((double)xIdx / 2.0) * gridSpacingLongitudinal);
                                    double latitude = gridOriginLatitude + ((double)yIdx * gridSpacingLatitudinal);

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
                                        Id = $"Sid_{xIdx}_{yIdx}",
                                        FeatureName = new FeatureName[] { new FeatureName { DisplayName = $"Sid_{xIdx}_{yIdx}", } },
                                        Value = depth,
                                        Geometry = geometry
                                    };

                                    geoFeatures.Add(sounding);
                                }
                            }

                            Progress?.Invoke(50 + (int)((50.0 / (double)numPointsLatitude) * (double)yIdx));
                        }

                        dataPackage.RawHdfData = hdf5S102Root;
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
