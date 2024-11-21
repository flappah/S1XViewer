using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types.Interfaces;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using GeoAPI.Geometries;
using Esri.ArcGISRuntime.Geometry;
using System.Globalization;
using System.Windows.Media.Animation;

namespace S1XViewer.Model
{
    public abstract class DataParserBase : IDataParser
    {
        protected IOptionsStorage _optionsStorage;
        protected SynchronizationContext? _syncContext;

        public abstract event IDataParser.ProgressFunction? Progress;

        /// <summary>
        ///     Retrieves the srsName attribute from somewhere in the Xmldocument and makes sure it is a numerical value
        /// </summary>
        /// <param name="node">start node to look in</param>
        /// <returns>string representing the coordinate reference system</returns>
        protected string GetSrsName(XmlNode node)
        {
            string defaultCRSString;
            var srsNode = node.SelectSingleNode("//*[@srsName]");
            if (srsNode != null)
            {
                defaultCRSString = srsNode.Attributes["srsName"].InnerText;
                if (defaultCRSString.Contains(":"))
                {
                    defaultCRSString = defaultCRSString.LastPart(":");
                }
            }
            else
            {
                defaultCRSString = _optionsStorage.Retrieve("comboBoxCRS");
            }

            if (String.IsNullOrEmpty(defaultCRSString) == true || defaultCRSString.IsNumeric() == false)
            {
                defaultCRSString = "4326"; // wgs84 is default
            }

            return defaultCRSString;
        }

        /// <summary>
        ///     Finds the UTM zone if applicable
        /// </summary>
        /// <param name="crsId">CRS id</param>
        /// <returns>UTM zone including north or sound. Empty is no UTM</returns>
        protected string FindUtmZone(int crsId)
        {
            var assemblyPath = Assembly.GetAssembly(GetType())?.Location ?? "";
            string folder = Path.GetDirectoryName(assemblyPath) ?? "";

            try
            {
                string[] allCrs = File.ReadAllLines($@"{folder}\crs.csv");
                var crsLines = allCrs.ToList().Where(q => q.Contains(crsId + ",")).ToList();
                if (crsLines != null && crsLines.Count > 0)
                {
                    string crsLine = crsLines.Last();
                    if (crsLine.Contains("UTM"))
                    {
                        var utmZone = crsLine.Substring(crsLine.IndexOf("UTM") + 3, crsLine.Length - crsLine.IndexOf("UTM") - 3).Trim().Replace("Zone", "").Replace("zone", "");
                        return utmZone;
                    }
                }
            }
            catch (Exception) { }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="utmZone"> </param>
        /// <returns></returns>
        protected Coordinate TransformUTMToWGS84(Coordinate point, string utmZone)
        {
            CoordinateSystemFactory csFact = new CoordinateSystemFactory();
            CoordinateTransformationFactory ctFact = new CoordinateTransformationFactory();

            string wgs84SpatialRefWKT =
                @"GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137,298.257223563]],PRIMEM[""Greenwich"",0],UNIT[""Degree"",0.0174532925199433]]";

            ICoordinateSystem wgs84 = csFact.CreateFromWkt(wgs84SpatialRefWKT);

            if (int.TryParse(utmZone.ToUpper().Replace("N", "").Replace("S", ""), out int utmNumber))
            {
                IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmNumber, utmZone.ToUpper().Contains("N"));
                ICoordinateTransformation trans = ctFact.CreateFromCoordinateSystems(utm, wgs84);

                Coordinate tCoord = trans.MathTransform.Transform(point);
                return tCoord;
            }

            throw new InvalidCastException($"Could not convert zone '{utmZone}' to int!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected Coordinate[] TransformListUTMToWGS84(Coordinate[] points, string utmZone)
        {
            CoordinateSystemFactory csFact = new CoordinateSystemFactory();
            CoordinateTransformationFactory ctFact = new CoordinateTransformationFactory();

            string wgs84SpatialRefWKT =
                @"GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137,298.257223563]],PRIMEM[""Greenwich"",0],UNIT[""Degree"",0.0174532925199433]]";

            ICoordinateSystem wgs84 = csFact.CreateFromWkt(wgs84SpatialRefWKT);

            if (int.TryParse(utmZone.ToUpper().Replace("N", "").Replace("S", ""), out int utmNumber))
            {
                IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmNumber, utmZone.ToUpper().Contains("N"));
                ICoordinateTransformation trans = ctFact.CreateFromCoordinateSystems(utm, wgs84);

                Coordinate[] tpoints = trans.MathTransform.TransformList(points).ToArray();
                return tpoints;
            }

            throw new InvalidCastException($"Could not convert zone '{utmZone}' to int!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        protected XmlNamespaceManager GetAllNamespaces(XmlDocument xDoc)
        {
            XmlNamespaceManager result = new XmlNamespaceManager(xDoc.NameTable);

            IDictionary<string, string> localNamespaces = null;
            XPathNavigator xNav = xDoc.CreateNavigator();
            while (xNav.MoveToFollowing(XPathNodeType.Element))
            {
                localNamespaces = xNav.GetNamespacesInScope(XmlNamespaceScope.Local);
                foreach (var localNamespace in localNamespaces)
                {
                    string prefix = localNamespace.Key;
                    if (string.IsNullOrEmpty(prefix))
                        prefix = "DEFAULT";

                    result.AddNamespace(prefix, localNamespace.Value);
                }
            }

            return result;
        }
        
        /// <summary>
        ///     Parses specified XMLDocument. Async version
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>
        public abstract Task<IS1xxDataPackage> ParseAsync(System.Xml.XmlDocument xmlDocument);

        /// <summary>
        ///     Parses specified HDF5 file. Async version
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        public abstract Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime);

        /// <summary>
        /// Parses specified XMLDocument. 
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>

        public abstract IS1xxDataPackage Parse(System.Xml.XmlDocument xmlDocument);

        /// <summary>
        ///     Parses specified HDF5 file
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        public abstract IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime);

        /// <summary>
        /// Resolves specified links by looking in the specified lists for the requested ID's
        /// </summary>
        /// <param name="links">ILink[]</param>
        /// <param name="informationFeatures">List<IInformationFeature></param>
        /// <param name="metaFeatures">List<IMetaFeature></param>
        /// <param name="geoFeatures">List<IGeoFeature></param>
        protected void ResolveLinks(ILink[] links, List<IInformationFeature> informationFeatures, List<IMetaFeature> metaFeatures, List<IGeoFeature> geoFeatures)
        {
            if (links == null)
            {
                return;
            }

            foreach (ILink link in links)
            {
                int foundInfoFeatureIndex =
                    informationFeatures.FindIndex(ftr =>
                        !String.IsNullOrEmpty(ftr.Id) &&
                        ftr.Id.Contains(link.Href.Replace("#", "")));

                if (foundInfoFeatureIndex != -1)
                {
                    link.Offset = $"I_{foundInfoFeatureIndex}";
                    link.LinkedFeature = informationFeatures[foundInfoFeatureIndex].DeepClone();
                }
                else
                {
                    int foundMetaFeatureIndex =
                        metaFeatures.FindIndex(ftr =>
                            !String.IsNullOrEmpty(ftr.Id) &&
                            ftr.Id.Contains(link.Href.Replace("#", "")));

                    if (foundMetaFeatureIndex != -1)
                    {
                        link.Offset = $"M_{foundMetaFeatureIndex}";
                        link.LinkedFeature = metaFeatures[foundMetaFeatureIndex].DeepClone();
                    }
                    else
                    {
                        int foundGeoFeatureIndex =
                            geoFeatures.FindIndex(ftr =>
                                !String.IsNullOrEmpty(ftr.Id) &&
                                ftr.Id.Contains(link.Href.Replace("#", "")));

                        if (foundGeoFeatureIndex != -1)
                        {
                            link.Offset = $"G_{foundGeoFeatureIndex}";
                            link.LinkedFeature = geoFeatures[foundGeoFeatureIndex].DeepClone();
                        }
                    }
                }
            }
        }
    }
}
