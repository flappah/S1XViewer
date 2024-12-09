using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class PointBuilder : GeometryBuilderBase, IPointBuilder
    {
        /// <summary>
        ///     For injection purposes
        /// </summary>
        public PointBuilder()
        {
        }

        /// <summary>
        ///     Calculates the geometry from the provided positions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="srs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Esri.ArcGISRuntime.Geometry.Geometry FromPositions(double[] x, double[] y, int srs = -1)
        {
            if (x is null || x.Length == 0)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y is null || y.Length == 0)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (srs != -1)
            {
                _spatialReferenceSystem = srs;
            }
            else
            {
                if (string.IsNullOrEmpty(DefaultCRS) == false)
                {
                    if (int.TryParse(DefaultCRS, out int defaultCRSValue))
                    {
                        _spatialReferenceSystem = defaultCRSValue; // if no srsNode is found assume default reference systema
                    }
                    else
                    {
                        _spatialReferenceSystem = 4326; // since most S1xx standards assume WGS84 is default, use this is the uber default CRS
                    }
                }
                else
                {
                    _spatialReferenceSystem = 4326;// since most S1xx standards assume WGS84 is default, use this is the uber default CRS
                }
            }

            if (InvertLonLat)
            {
                return new MapPoint(y[0], x[0], SpatialReference.Create(_spatialReferenceSystem));
            }
            else
            {
                return new MapPoint(x[0], y[0], SpatialReference.Create(_spatialReferenceSystem));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="srs"></param>
        /// <returns></returns>
        public override Esri.ArcGISRuntime.Geometry.Geometry FromPositions(double[] x, double[] y, double z, int srs = -1)
        {
            if (x is null || x.Length == 0)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y is null || y.Length == 0)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (srs != -1)
            {
                _spatialReferenceSystem = srs;
            }
            else
            {
                if (string.IsNullOrEmpty(DefaultCRS) == false)
                {
                    if (int.TryParse(DefaultCRS, out int defaultCRSValue))
                    {
                        _spatialReferenceSystem = defaultCRSValue; // if no srsNode is found assume default reference systema
                    }
                    else
                    {
                        _spatialReferenceSystem = 4326; // since most S1xx standards assume WGS84 is default, use this is the uber default CRS
                    }
                }
                else
                {
                    _spatialReferenceSystem = 4326;// since most S1xx standards assume WGS84 is default, use this is the uber default CRS
                }
            }

            if (InvertLonLat)
            {
                return new MapPoint(y[0], x[0], z, SpatialReference.Create(_spatialReferenceSystem));
            }
            else
            {
                return new MapPoint(x[0], y[0], z, SpatialReference.Create(_spatialReferenceSystem));
            }
        }

        /// <summary>
        ///     Retrieves the geometry from the specified Xml Node
        /// </summary>
        /// <param name="node">node containing a basic geometry</param>
        /// <param name="mgr">namespace manager</param>
        /// <returns>ESRI Arc GIS geometry</returns>
        public override Esri.ArcGISRuntime.Geometry.Geometry? FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node is null || node.HasChildNodes == false)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (mgr is null)
            {
                throw new ArgumentNullException(nameof(mgr));
            }

            XmlNode? srsNode = null;
            if (node?.Attributes?.Count > 0 && node.Attributes.Contains("srsName") == true)
            {
                srsNode = node;
            }
            else if (node?.FirstChild?.Attributes?.Count > 0 && node.FirstChild.Attributes.Contains("srsName") == true)
            {
                srsNode = node.FirstChild;
            }

            if (srsNode != null)
            {
                if (int.TryParse(srsNode.Attributes.Find("srsName")?.Value.ToString().LastPart(char.Parse(":")), out int refSystem) == false)
                {
                    if (int.TryParse(srsNode.Attributes.Find("srsName")?.Value.ToString().LastPart(char.Parse("/")), out refSystem) == false)
                    { 
                        refSystem = 0; 
                    }
                }
                _spatialReferenceSystem = refSystem;
            }

            if (_spatialReferenceSystem == 0)
            {
                if (string.IsNullOrEmpty(DefaultCRS) == false)
                {
                    if (int.TryParse(DefaultCRS, out int defaultCRSValue))
                    {
                        _spatialReferenceSystem = defaultCRSValue; // if no srsNode is found assume default reference systema
                    }
                    else
                    {
                        _spatialReferenceSystem = 4326; // since most S1xx standards assume WGS84 is default, use this is the uber default CRS
                    }
                }
                else
                {
                    _spatialReferenceSystem = 4326;// since most S1xx standards assume WGS84 is default, use this is the uber default CRS
                }
            }

            var pointNode = node.FirstChild;
            if (pointNode != null && pointNode.HasChildNodes && pointNode.FirstChild.Name.ToUpper().Contains("POS"))
            {
                var splittedPosition =
                    pointNode.FirstChild.InnerText.Replace("\t", " ").Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                if (!double.TryParse(splittedPosition[0], NumberStyles.Float, new CultureInfo("en-US"), out double x))
                {
                    x = 0.0;
                }
                if (!double.TryParse(splittedPosition[1], NumberStyles.Float, new CultureInfo("en-US"), out double y))
                {
                    y = 0.0;
                }

                if (InvertLonLat)
                {
                    return new MapPoint(y, x, SpatialReference.Create(_spatialReferenceSystem));
                }
                else
                {
                    return new MapPoint(x, y, SpatialReference.Create(_spatialReferenceSystem));
                }
            }

            return null;
        }
    }
}
