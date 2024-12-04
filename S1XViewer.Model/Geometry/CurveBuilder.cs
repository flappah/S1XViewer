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

            var segmentNodes = node.FirstChild?.SelectNodes("gml:segments", mgr);
            if (segmentNodes != null && segmentNodes.Count > 0)
            {
                var segments = new List<List<MapPoint>>();
                foreach (XmlNode segmentNode in segmentNodes)
                {
                    var curveMapPoints = new List<MapPoint>();
                    var lineStringNodes = segmentNode.ChildNodes;
                    foreach (XmlNode lineStringNode in lineStringNodes)
                    {
                        var posListNode = lineStringNode.SelectSingleNode("gml:posList", mgr);
                        if (posListNode != null)
                        {
                            string[] splittedPositionsArray =
                                lineStringNode.ChildNodes[0].InnerText
                                    .Replace("\t", " ")
                                    .Replace("\n", " ")
                                    .Replace("\r", " ")
                                    .Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < splittedPositionsArray.Length; i += 2)
                            {
                                if (!double.TryParse(splittedPositionsArray[i], NumberStyles.Float, new CultureInfo("en-US"), out double x))
                                {
                                    x = 0.0;
                                }
                                if (!double.TryParse(splittedPositionsArray[i + 1], NumberStyles.Float, new CultureInfo("en-US"), out double y))
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
                        else
                        {
                            var posNodes = lineStringNode.SelectNodes("gml:pos", mgr);
                            if (posNodes != null && posNodes.Count > 0)
                            {
                                var splittedPositionsList = new List<string>();
                                foreach (XmlNode posNode in posNodes)
                                {
                                    string[] splittedPosition =
                                        posNode.InnerText
                                            .Replace("\t", " ")
                                            .Replace("\n", " ")
                                            .Replace("\r", " ")
                                            .Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                                    if (splittedPosition.Length == 2)
                                    {
                                        splittedPositionsList.AddRange(splittedPosition);
                                    }
                                }

                                var splittedPositionsArray = splittedPositionsList.ToArray();
                                for (int i = 0; i < splittedPositionsArray.Length; i += 2)
                                {
                                    if (!double.TryParse(splittedPositionsArray[i], NumberStyles.Float, new CultureInfo("en-US"), out double x))
                                    {
                                        x = 0.0;
                                    }
                                    if (!double.TryParse(splittedPositionsArray[i + 1], NumberStyles.Float, new CultureInfo("en-US"), out double y))
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
                    }

                    segments.Add(curveMapPoints);
                }

                if (segments.Count > 0)
                {
                    if (IsPositionInAnySegmentInverted(segments))
                    {
                        segments = InvertPositionsInSegments(segments);
                        InvertLonLat = true;
                    }

                    var polyline = new Polyline(segments, SpatialReference.Create(_spatialReferenceSystem));
                    return polyline;
                }
            }

            return null;
        }

        /// <summary>
        ///     Method tries to resolve if any position in the supplied segments is inverted
        ///     Now this method is by no means complete. It only does a basic check if the 
        ///     Y variable is larger than 90.0 which means it HAS to be a longitude. If so
        ///     true is returned
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        private bool IsPositionInAnySegmentInverted(List<List<MapPoint>> segments)
        {
            foreach (List<MapPoint> interiorMapPoints in segments)
            {
                foreach (MapPoint mapPoint in interiorMapPoints)
                {
                    if (mapPoint.Y > 90.0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Invert the surface
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        private List<List<MapPoint>> InvertPositionsInSegments(List<List<MapPoint>> segments)
        {
            var invertedSegments = new List<List<MapPoint>>();

            foreach (List<MapPoint> interiorMapPoints in segments)
            {
                var interiorSegment = new List<MapPoint>();
                foreach (MapPoint mapPoint in interiorMapPoints)
                {
                    if (mapPoint.Y > 90.0)
                    {
                        interiorSegment.Add(new MapPoint(mapPoint.Y, mapPoint.X));
                    }
                    else
                    {
                        interiorSegment.Add(new MapPoint(mapPoint.X, mapPoint.Y));
                    }
                }

                invertedSegments.Add(interiorSegment);
            }

            return invertedSegments;
        }
    }
}
