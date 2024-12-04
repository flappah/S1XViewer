using HDF5CSharp.DataTypes;
using NetTopologySuite.Algorithm;
using OSGeo.GDAL;
using OSGeo.OSR;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Features;
using S1XViewer.Types.Interfaces;
using System.Globalization;
using System.Security.Principal;
using System.Windows;
using System.Xml;

namespace S1XViewer.Model
{
    public class S104DCF2V20DataParser : HdfDataParserBase, IS104DCF2V20DataParser
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
        public S104DCF2V20DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS111ProductSupport productSupport, IOptionsStorage optionsStorage)
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
        private void CreateTiff(S104DataPackage dataPackage)
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
        ///     Parses specified HDF5 file
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        public override async Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime)
        {
            if (string.IsNullOrEmpty(hdf5FileName))
            {
                throw new ArgumentException($"'{nameof(hdf5FileName)}' cannot be null or empty.", nameof(hdf5FileName));
            }

            if (selectedDateTime == null)
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

            var dataPackage = new S104DataPackage
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

                Hdf5Element? waterLevelFeatureInstanceNode = null;
                Hdf5Element? group = null;
                foreach (Hdf5Element? node in featureElement.Children)
                {
                    foreach (Hdf5Element? childGroup in node.Children)
                    {
                        var timePointString = childGroup.Attributes.Find("timePoint")?.Value<string>() ?? string.Empty;
                        if (DateTime.TryParseExact(timePointString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timePoint))
                        {
                            timePoint = timePoint.ToUniversalTime();
                            if (timePoint.Equals(selectedDateTime))
                            {
                                waterLevelFeatureInstanceNode = node;
                                group = childGroup;
                                break;
                            }
                        }
                    }

                    if (waterLevelFeatureInstanceNode != null && group != null)
                    {
                        break;
                    }
                }

                if (waterLevelFeatureInstanceNode != null && group != null && waterLevelFeatureInstanceNode.Name.Contains("/WaterLevel."))
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

                    var waterLevelFeatureNode = (Hdf5Element)waterLevelFeatureInstanceNode.Parent;
                    if (waterLevelFeatureNode == null)
                    {
                        return;
                    }

                    var scanDirection = waterLevelFeatureNode.Attributes.Find("sequencingRule.scanDirection")?.Value<string>() ?? string.Empty;

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

                        dataPackage.minX = gridOriginLongitude;
                        dataPackage.minY = gridOriginLatitude;
                        dataPackage.maxX = eastBoundLongitude;
                        dataPackage.maxY = northBoundLatitude;
                        dataPackage.dY = gridSpacingLongitudinal;
                        dataPackage.dX = gridSpacingLatitudinal;
                        dataPackage.noDataValue = nillValueHeight;
                        dataPackage.numPointsX = numPointsLongitude;
                        dataPackage.numPointsY = numPointsLatitude;
                        dataPackage.maxDataValue = -999.0;
                        dataPackage.minDataValue = 999.0;

                        dataPackage.Data = new float[numPointsLatitude, numPointsLongitude];

                        for (int yIdx = 0; yIdx < numPointsLatitude; yIdx++)
                        {
                            for (int xIdx = 0; xIdx < (numPointsLongitude * 3); xIdx += 3)
                            {
                                if (currentDataset.values[yIdx, xIdx] != nillValueHeight)
                                {
                                    // find out min/max values
                                    if (currentDataset.values[yIdx, xIdx] < dataPackage.minDataValue)
                                    {
                                        dataPackage.minDataValue = currentDataset.values[yIdx, xIdx];
                                    }
                                    if (currentDataset.values[yIdx, xIdx] > dataPackage.maxDataValue)
                                    {
                                        dataPackage.maxDataValue = currentDataset.values[yIdx, xIdx];
                                    }

                                    float value = currentDataset.values[yIdx, xIdx]; // actual data value
                                    dataPackage.Data[yIdx, (int)xIdx / 3] = value;
                                }
                                else
                                {
                                    dataPackage.Data[yIdx, (int)xIdx / 3] = (float) nillValueHeight;
                                }
                            }
                        }

                        CreateTiff(dataPackage);

                        dataPackage.RawHdfData = hdf5S111Root;
                        if (geoFeatures.Count > 0)
                        {
                            dataPackage.GeoFeatures = geoFeatures.ToArray();
                            dataPackage.MetaFeatures = new IMetaFeature[0];
                            dataPackage.InformationFeatures = new IInformationFeature[0];
                        }

                        oldTimePoint = timePoint;
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
        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            return ParseAsync(hdf5FileName, selectedDateTime).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     No implementation!
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
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
