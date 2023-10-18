using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class CurveBuilder : GeometryBuilderBase, ICurveBuilder
    {
        /// <summary>
        ///     For injection purposes
        /// </summary>
        public CurveBuilder()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Esri.ArcGISRuntime.Geometry.Geometry FromPositions(double[] x, double[] y, int srs = -1)
        {
            throw new NotImplementedException();
        }

        public override Esri.ArcGISRuntime.Geometry.Geometry FromPositions(double[] x, double[] y, double z, int srs = -1)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Retrieves the geometry from the specified Xml Node
        /// </summary>
        /// <param name="node">node containing a basic geometry</param>
        /// <param name="mgr">namespace manager</param>
        /// <returns>ESRI Arc GIS geometry</returns>
        public override Esri.ArcGISRuntime.Geometry.Geometry? FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (mgr is null)
            {
                throw new ArgumentNullException(nameof(mgr));
            }

            if (node?.Attributes?.Count > 0 && node.Attributes.Contains("nilReason") == true)
            {
                if (String.IsNullOrEmpty(node.Attributes["nilReason"]?.InnerText ?? "") == false)
                {
                    return null;
                }
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
                if (!int.TryParse(srsNode.Attributes.Find("srsName")?.Value.ToString().LastPart(char.Parse(":")), out int refSystem))
                {
                    refSystem = 0;
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

            var segmentNodes = node.FirstChild.SelectNodes("gml:segments", mgr);
            if (segmentNodes != null && segmentNodes.Count > 0)
            {
                var segments = new List<List<MapPoint>>();
                foreach (XmlNode segmentNode in segmentNodes)
                {
                    var curveMapPoints = new List<MapPoint>();
                    var lineStringNodes = segmentNode.ChildNodes;
                    foreach (XmlNode lineStringNode in lineStringNodes)
                    {
                        if (lineStringNode.HasChildNodes &&
                            lineStringNode.ChildNodes[0].Name.ToUpper().Contains("POSLIST"))
                        {
                            string[] splittedPositionArray =
                                lineStringNode.ChildNodes[0].InnerText
                                    .Replace("\t", " ")
                                    .Replace("\n", " ")
                                    .Replace("\r", " ")
                                    .Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < splittedPositionArray.Length; i += 2)
                            {
                                if (!double.TryParse(splittedPositionArray[i], NumberStyles.Float, new CultureInfo("en-US"), out double x))
                                {
                                    x = 0.0;
                                }
                                if (!double.TryParse(splittedPositionArray[i + 1], NumberStyles.Float, new CultureInfo("en-US"), out double y))
                                {
                                    y = 0.0;
                                }

                                if (InvertLonLat)
                                {
                                    curveMapPoints.Add(new MapPoint(y, x, SpatialReference.Create(_spatialReferenceSystem)));
                                }
                                else
                                {
                                    curveMapPoints.Add(new MapPoint(x, y, SpatialReference.Create(_spatialReferenceSystem)));
                                }
                            }
                        }
                    }

                    segments.Add(curveMapPoints);
                }

                if (segments.Count > 0)
                {
                    var polyline = new Polyline(segments, SpatialReference.Create(_spatialReferenceSystem));
                    return polyline;
                }
            }

            return null;
        }
    }
}
