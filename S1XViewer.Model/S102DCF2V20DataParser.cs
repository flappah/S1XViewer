using HDF5CSharp.DataTypes;
using OSGeo.GDAL;
using OSGeo.OSR;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System.Globalization;
using System.Security.Principal;
using System.Windows;
using System.Xml;

namespace S1XViewer.Model
{
    public class S102DCF2V20DataParser : HdfDataParserBase, IS102DCF2V20DataParser
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
        public S102DCF2V20DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS102ProductSupport productSupport, IOptionsStorage optionsStorage)
        {
            _datasetReader = datasetReader;
            _geometryBuilderFactory = geometryBuilderFactory;
            _productSupport = productSupport;
            _optionsStorage = optionsStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataPackage"></param>
        private void CreateTiff(S102DataPackage dataPackage)
        {
            var tempPath = Path.GetTempPath();
            GdalConfiguration.ConfigureGdal();

            string tiffFileName = $"{tempPath}s1xv_{Guid.NewGuid()}.tif";

            var directoryInfo = new DirectoryInfo(tempPath);
            var directorySecurity = directoryInfo.GetAccessControl();

            directorySecurity.SetOwner(WindowsIdentity.GetCurrent().User);
            directoryInfo.SetAccessControl(directorySecurity);
            directoryInfo.Attributes = FileAttributes.Normal;

            if (File.Exists(tiffFileName) == true)
            {
                var fileInfo = new FileInfo(tiffFileName);
                var fileSecurity = fileInfo.GetAccessControl();

                fileSecurity.SetOwner(WindowsIdentity.GetCurrent().User);
                fileInfo.SetAccessControl(fileSecurity);
                fileInfo.IsReadOnly = false;                

                try
                {
                    File.Delete(tiffFileName);
                }
                catch (Exception) { }
            }

            string line = "";
            try
            {
                Driver driver = Gdal.GetDriverByName("GTiff");
                using (Dataset outImageDs = driver.Create(tiffFileName, dataPackage.numPointsX, dataPackage.numPointsY, 1, DataType.GDT_Float32, null))
                {
                    string wktProj;
                    Osr.GetWellKnownGeogCSAsWKT("WGS84", out wktProj);
                    outImageDs.SetProjection(wktProj);

                    var mapWidth = dataPackage.maxX - dataPackage.minX;
                    var mapHeight = dataPackage.maxY - dataPackage.minY;
                    double[] geoTransform = new double[] { dataPackage.minX, mapWidth / dataPackage.numPointsX, 0, dataPackage.maxY, 0, (mapHeight / dataPackage.numPointsY) * (-1) };

                    outImageDs.SetGeoTransform(geoTransform);

                    Band outBand = outImageDs.GetRasterBand(1);
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
                    outImageDs.FlushCache();
                    outBand.Dispose();
                    outImageDs.Dispose();

                    File.SetAttributes(tiffFileName, FileAttributes.Normal);
                }
            }
            catch (Exception ex)
            {
                var exception = ex;
                var errorText = string.Empty;
                while (exception != null)
                {
                    errorText += $"\n{exception.Source}\n{exception.Message}\n{exception.StackTrace}";
                    exception = exception.InnerException; 
                }

                MessageBox.Show($"({line}) Can't write file {tiffFileName}.{errorText}");
            }
            finally
            {
                dataPackage.TiffFileName = tiffFileName;
            }
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
                Id = Guid.NewGuid(),
                FileName = hdf5FileName,
                Type = S1xxTypes.S102,
                RawHdfData = null
            };

            _syncContext = SynchronizationContext.Current;
            Progress?.Invoke(50);

            Hdf5Element hdf5S102Root = await _productSupport.RetrieveHdf5FileAsync(hdf5FileName);
            (long horizontalCRS, string utmZone) = RetrieveHorizontalCRS(hdf5S102Root, hdf5FileName);
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

            Hdf5Element? featureElement = hdf5S102Root.Children.Find(elm => elm.Name.Equals("/BathymetryCoverage"));
            if (featureElement == null)
            {
                return dataPackage;
            }

            // check position ordering lon/lat vs lat/lon
            var axisNameElement = featureElement.Children.Find(elm => elm.Name.Equals("/BathymetryCoverage/axisNames"));
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
            var eastBoundLongitude = hdf5S102Root.Attributes.Find("eastBoundLongitude")?.Value<double>(0f) ?? 0f;
            var northBoundLatitude = hdf5S102Root.Attributes.Find("northBoundLatitude")?.Value<double>(0f) ?? 0f;
            var southBoundLatitude = hdf5S102Root.Attributes.Find("southBoundLatitude")?.Value<double>(0f) ?? 0f;
            var westBoundLongitude = hdf5S102Root.Attributes.Find("westBoundLongitude")?.Value<double>(0f) ?? 0f;

            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int)horizontalCRS);

            // retrieve values for undefined data cells
            double nillValueDepth = 1000000;
            double nillValueUncertainty = 1000000;
            var featureMetaInfoElements = _datasetReader.ReadCompound<BathymetryCoverageInformationInstance>(hdf5FileName, "/Group_F/BathymetryCoverage");
            if (featureMetaInfoElements.values.Length > 0)
            {
                foreach (var featureMetainfoElementValue in featureMetaInfoElements.values)
                {
                    if (featureMetainfoElementValue.code.Equals("depth"))
                    {
                        if (float.TryParse(featureMetainfoElementValue.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out float depthValue))
                        {
                            nillValueDepth = depthValue;
                        }
                    }
                    else if (featureMetainfoElementValue.code.Equals("uncertainty"))
                    {
                        if (float.TryParse(featureMetainfoElementValue.fillValue, NumberStyles.Float, new CultureInfo("en-US"), out float uncertaintyValue))
                        {
                            nillValueUncertainty += uncertaintyValue;
                        }
                    }
                }
            }

            // first element is relevant group of data
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
                        var gridOriginLatitude = minGroupParentNode.Attributes.Find("gridOriginLatitude")?.Value<double>() ?? -999.0;
                        var gridOriginLongitude = minGroupParentNode.Attributes.Find("gridOriginLongitude")?.Value<double>() ?? -999.0;

                        var gridSpacingLatitudinal = minGroupParentNode.Attributes.Find("gridSpacingLatitudinal")?.Value<double>() ?? -999.0;
                        var gridSpacingLongitudinal = minGroupParentNode.Attributes.Find("gridSpacingLongitudinal")?.Value<double>() ?? -999.0;

                        if (gridOriginLatitude == -999.0 || gridOriginLongitude == -999.0 || gridSpacingLatitudinal == -999.0 || gridSpacingLongitudinal == -999.0)
                        {
                            return;
                        }

                        var startSequence = minGroupParentNode.Attributes.Find("startSequence")?.Value<string>() ?? String.Empty;

                        // Now this whole algorithm still doesn't take startSequence into account!! In fact it always assumes that the gridOrigins are at lower left!!! 
                        // This should be changed in the future. For now startSequence is only evaluated to calculated the true origin since S102 specifies the 
                        // grid origin to be at the center point of the grid cell

                        if (startSequence == "0,0")
                        {
                            // in older versions of the S102 it's not necessary to apply the shift!

                            var southBoundLatitudeInstance = minGroupParentNode.Attributes.Find("southBoundLatitude")?.Value<double>(0f) ?? 0f;
                            var westBoundLongitudeInstance = minGroupParentNode.Attributes.Find("westBoundLongitude")?.Value<double>(0f) ?? 0f;

                            if (westBoundLongitudeInstance != gridOriginLongitude &&
                                southBoundLatitudeInstance != gridOriginLatitude)
                            {
                                gridOriginLatitude -= (gridSpacingLatitudinal * 0.5);
                                gridOriginLongitude -= (gridSpacingLongitudinal * 0.5);
                            }
                        }

                        // now check if input is UTM. If so transform to decimal degrees. 
                        if (String.IsNullOrEmpty(utmZone) == false)
                        {
                            GeoAPI.Geometries.Coordinate transformedCoordinate =
                                TransformUTMToWGS84(new GeoAPI.Geometries.Coordinate(gridOriginLongitude, gridOriginLatitude), utmZone);

                            gridOriginLongitude = transformedCoordinate.X;
                            gridOriginLatitude = transformedCoordinate.Y;
                        }

                        var numPointsLatitudinalElement = minGroupParentNode.Attributes.Find("numPointsLatitudinal");
                        int numPointsLatitude = numPointsLatitudinalElement?.Value<int>() ?? -1;
                        var numPointsLongitudinalNode = minGroupParentNode.Attributes.Find("numPointsLongitudinal");
                        int numPointsLongitude = numPointsLongitudinalNode?.Value<int>() ?? -1;

                        if (numPointsLatitude == -1 || numPointsLongitude == -1)
                        {
                            return;
                        }

                        // apparently it sometimes happens that there's something else besides a Values element in the Group_Xxx container. 
                        Hdf5Element valuesElement = minGroup.Children[0];
                        if (valuesElement != null && valuesElement.Name.ToUpper().Contains("/VALUES") == false)
                        {
                            int i = 0;
                            while (i++ < minGroup.Children.Count && valuesElement.Name.ToUpper().Contains("/VALUES") == false)
                            {
                                valuesElement = minGroup.Children[i];
                            }
                        }

                        if (valuesElement == null || valuesElement?.Name.ToUpper().Contains("/VALUES") == false)
                        {
                            return;
                        }

                        var depthsDataset =
                              _datasetReader.ReadArrayOfFloats(hdf5FileName, valuesElement?.Name, numPointsLatitude, numPointsLongitude * 2);

                        if (depthsDataset.members.Length == 0)
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
                                // TODO: add option to invert axis!
                                dataPackage.Data[yIdx, (int)xIdx / 2] = depthsDataset.values[yIdx, xIdx];
                            }
                        }

                        CreateTiff(dataPackage);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            return ParseAsync(hdf5FileName, selectedDateTime).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            return new S102DataPackage
            {
                Type = S1xxTypes.Null,
                RawHdfData = null,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public override async Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            return new S102DataPackage
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
