using HDF5CSharp.DataTypes;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using System.Reflection;

namespace S1XViewer.Model
{
    public abstract class HdfDataParserBase : DataParserBase, IHdfDataParserBase
    {
        protected IProductSupportBase? _productSupport;

        /// <summary>
        ///     Retrieves the horizontal CRS
        /// </summary>
        /// <param name="hdf5S111Root"></param>
        /// <param name="hdf5FileName"></param>
        /// <returns>crs-id, zone (if utm)</returns>
        protected (int crs, string zone) RetrieveHorizontalCRS(Hdf5Element hdf5S111Root, string hdf5FileName)
        {
            string utmZone = "";
            int horizontalCRS = -1;
            var horizontalCRSAttribute = hdf5S111Root.Attributes.Find("horizontalCRS"); // S111 v1.2
            if (horizontalCRSAttribute != null)
            {
                horizontalCRS = horizontalCRSAttribute?.Value<int>() ?? -1;
                if (horizontalCRS == -1)
                {
                    var nameOfHorizontalCRSAttribute = hdf5S111Root.Attributes.Find("nameOfHorizontalCRS");
                    var nameOfHorizontalCRS = nameOfHorizontalCRSAttribute?.Value<string>() ?? string.Empty;

                    short typeOfHorizontalCRS = _productSupport.GetTypeOfHorizontalCRS(hdf5FileName);
                    if (typeOfHorizontalCRS == 1 || typeOfHorizontalCRS == 2)
                    {
                        var horizontalCSAttribute = hdf5S111Root.Attributes.Find("horizontalCS");
                        var horizontalCS = horizontalCSAttribute?.Value<int>() ?? -1;

                        if (horizontalCS != -1 && horizontalCS.In(6422, 4400, 4500))
                        {
                            horizontalCRS = horizontalCS;
                        }
                        else
                        {
                            var horizontalDatumAttribute = hdf5S111Root.Attributes.Find("horizontalDatum");
                            int horizontalDatum = horizontalDatumAttribute?.Value<int>() ?? -1;

                            if (horizontalDatum != -1)
                            {
                                horizontalCRS = horizontalDatum;
                            }
                        }
                    }
                }
            }
            else
            {
                var horizontalDatumValueAttribute = hdf5S111Root.Attributes.Find("horizontalDatumValue"); // S111 v1.1
                var horizontalDatumValue = horizontalDatumValueAttribute?.Value<Int32>() ?? 0;

                if (horizontalDatumValue > 0)
                {
                    var epochAttribute = hdf5S111Root.Attributes.Find("epoch");
                    var epoch = epochAttribute?.Value<string>() ?? string.Empty;

                    if (String.IsNullOrEmpty(epoch) == false)
                    {
                        horizontalCRS = CorrectCRSWithEpoch(horizontalDatumValue, epoch);
                    }
                    else
                    {
                        horizontalCRS = 4326; // a set utmZone means input data will be transformed to WGS84!
                        utmZone = FindUtmZone(horizontalDatumValue);
                    }
                }
            }

            return (horizontalCRS, utmZone);
        }

        /// <summary>
        ///     Retrieves the corrected CRS for baseId and epoch. File crs.csv contains all CRS's
        /// </summary>
        /// <param name="baseCRSId"></param>
        /// <param name="epoch"></param>
        /// <returns>Corrected CRS value</returns>
        protected int CorrectCRSWithEpoch(int baseCRSId, string epoch)
        {
            if (String.IsNullOrEmpty(epoch) == false)
            {
                var assemblyPath = Assembly.GetAssembly(GetType())?.Location ?? "";
                string folder = Path.GetDirectoryName(assemblyPath) ?? "";

                try
                {
                    string[] allCrs = File.ReadAllLines($@"{folder}\crs.csv");
                    var epochLines = allCrs.ToList().Where(q => q.Contains(epoch.ToUpper())).ToList();
                    if (epochLines != null && epochLines.Count > 0)
                    {
                        string epochLine = epochLines.Last();
                        var splittedEpochLine = epochLine.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        if (splittedEpochLine.Length == 2)
                        {
                            if (int.TryParse(splittedEpochLine[0], out int epochId))
                            {
                                return epochId;
                            }
                        }
                    }
                }
                catch (Exception) { }
            }

            return baseCRSId; // base WGS84
        }

        /// <summary>
        ///     Finds the UTM zone if applicable
        /// </summary>
        /// <param name="crsId">CRS id</param>
        /// <returns>UTM zone including north or sound. Empty is no UTM</returns>
        //protected string FindUtmZone(int crsId)
        //{
        //    var assemblyPath = Assembly.GetAssembly(GetType())?.Location ?? "";
        //    string folder = Path.GetDirectoryName(assemblyPath) ?? "";

        //    try
        //    {
        //        string[] allCrs = File.ReadAllLines($@"{folder}\crs.csv");
        //        var crsLines = allCrs.ToList().Where(q => q.Contains(crsId + ",")).ToList();
        //        if (crsLines != null && crsLines.Count > 0)
        //        {
        //            string crsLine = crsLines.Last();
        //            if (crsLine.Contains("UTM"))
        //            {
        //                var utmZone = crsLine.Substring(crsLine.IndexOf("UTM") + 3, crsLine.Length - crsLine.IndexOf("UTM") - 3).Trim().Replace("Zone", "").Replace("zone", "");
        //                return utmZone;
        //            }
        //        }
        //    }
        //    catch (Exception) { }

        //    return string.Empty;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="utmZone"> </param>
        /// <returns></returns>
        //protected Coordinate TransformUTMToWGS84(Coordinate point, string utmZone)
        //{
        //    CoordinateSystemFactory csFact = new CoordinateSystemFactory();
        //    CoordinateTransformationFactory ctFact = new CoordinateTransformationFactory();

        //    string wgs84SpatialRefWKT =
        //        @"GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137,298.257223563]],PRIMEM[""Greenwich"",0],UNIT[""Degree"",0.0174532925199433]]";

        //    ICoordinateSystem wgs84 = csFact.CreateFromWkt(wgs84SpatialRefWKT);

        //    if (int.TryParse(utmZone.ToUpper().Replace("N", "").Replace("S", ""), out int utmNumber))
        //    {
        //        IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmNumber, utmZone.ToUpper().Contains("N"));
        //        ICoordinateTransformation trans = ctFact.CreateFromCoordinateSystems(utm, wgs84);

        //        Coordinate tCoord = trans.MathTransform.Transform(point);
        //        return tCoord;
        //    }

        //    throw new InvalidCastException($"Could not convert zone '{utmZone}' to int!");
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        //protected Coordinate[] TransformListUTMToWGS84(Coordinate[] points, string utmZone)
        //{
        //    CoordinateSystemFactory csFact = new CoordinateSystemFactory();
        //    CoordinateTransformationFactory ctFact = new CoordinateTransformationFactory();

        //    string wgs84SpatialRefWKT =
        //        @"GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137,298.257223563]],PRIMEM[""Greenwich"",0],UNIT[""Degree"",0.0174532925199433]]";

        //    ICoordinateSystem wgs84 = csFact.CreateFromWkt(wgs84SpatialRefWKT);

        //    if(int.TryParse(utmZone.ToUpper().Replace("N", "").Replace("S", ""), out int utmNumber))
        //    {
        //        IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmNumber, utmZone.ToUpper().Contains("N"));
        //        ICoordinateTransformation trans = ctFact.CreateFromCoordinateSystems(utm, wgs84);

        //        Coordinate[] tpoints = trans.MathTransform.TransformList(points).ToArray();
        //        return tpoints; 
        //    }

        //    throw new InvalidCastException($"Could not convert zone '{utmZone}' to int!");
        //}
    }
}
