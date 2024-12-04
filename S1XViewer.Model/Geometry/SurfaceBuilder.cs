using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using System.Numerics;
using System.Security;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class SurfaceBuilder : GeometryBuilderBase, ISurfaceBuilder
    {
        /// <summary>
        ///     For injection purposes
        /// </summary>
        public SurfaceBuilder()
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="srs"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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

            var mappointList = new List<MapPoint>();

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
                for (int i = 0; i < x.Length; i++)
                {
                    mappointList.Add(new MapPoint(x[i], y[i], SpatialReference.Create(_spatialReferenceSystem)));
                }
            }
            else
            {
                for (int i = 0; i < x.Length; i++)
                {
                    mappointList.Add(new MapPoint(x[i], y[i], SpatialReference.Create(_spatialReferenceSystem)));
                }
            }

            return new Polygon(mappointList, SpatialReference.Create(_spatialReferenceSystem));
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

            var segments = new List<List<MapPoint>>();
            if (node?.HasChildNodes == true && node?.FirstChild?.HasChildNodes == true)
            {
                var surfaceNode = node.FirstChild;
                var patchesNodes = surfaceNode.SelectNodes(@"gml:patches", mgr);
                if (patchesNodes != null && patchesNodes.Count > 0)
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                    var spatialReferenceSystem = SpatialReference.Create(_spatialReferenceSystem);
                    foreach (XmlNode patchesNode in patchesNodes)
                    {
                        if (patchesNode.HasChildNodes)
                        {
                            var polygonPatchNode = patchesNode.FirstChild;
                            var exteriorNode = polygonPatchNode?.SelectSingleNode("gml:exterior", mgr);
                            if (exteriorNode != null && exteriorNode.HasChildNodes)
                            {
                                var exteriorMapPoints = new List<MapPoint>();
                                var linearRingNodes = exteriorNode.ChildNodes;
                                foreach (XmlNode linearRingNode in linearRingNodes)
                                {
                                    var posListNode = linearRingNode.SelectSingleNode("gml:posList", mgr);
                                    if (posListNode != null)
                                    {
                                        string[] splittedPositionsArray =
                                            posListNode.InnerText
                                                .Replace("\t", " ")
                                                .Replace("\n", " ")
                                                .Replace("\r", " ")
                                                .Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                                        if (((double)splittedPositionsArray.Length / 2.0) == Math.Abs(splittedPositionsArray.Length / 2.0))
                                        {
                                            var latitudes = new double[splittedPositionsArray.Length];
                                            var longitudes = new double[splittedPositionsArray.Length];

                                            Parallel.For(0, splittedPositionsArray.Length, index =>
                                            {
                                                // try to avoid this overload since this one is quite a bit slower than the simple TryParse!
                                                //if (double.TryParse(splittedPositionArray[index], NumberStyles.Float, new CultureInfo("en-US"), out double positionValue) == true)
                                                if (double.TryParse(
                                                    splittedPositionsArray[index].Replace(splittedPositionsArray[index].Contains('.') ? "." : ",", currentCulture.NumberFormat.NumberDecimalSeparator),
                                                    out double positionValue) == true)
                                                {
                                                    if (((BigInteger)index).IsEven)
                                                    {
                                                        latitudes[index] = positionValue;
                                                    }
                                                    else
                                                    {
                                                        longitudes[index] = positionValue;
                                                    }
                                                }
                                            });

                                            for (int i = 0; i < latitudes.Length; i += 2)
                                            {
                                                if (InvertLonLat)
                                                {
                                                    exteriorMapPoints.Add(new MapPoint(longitudes[i + 1], latitudes[i], spatialReferenceSystem));
                                                }
                                                else
                                                {
                                                    exteriorMapPoints.Add(new MapPoint(latitudes[i], longitudes[i + 1], spatialReferenceSystem));
                                                }
                                            }

                                            longitudes = null;
                                            latitudes = null;
                                        }
                                    }
                                    else
                                    {
                                        var posNodes = linearRingNode.SelectNodes("gml:pos", mgr);
                                        if (posNodes != null && posNodes.Count > 0)
                                        {
                                            var splittedPositionsList = new List<string>();
                                            foreach(XmlNode posNode in posNodes)
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
                                            if (((double)splittedPositionsArray.Length / 2.0) == Math.Abs(splittedPositionsArray.Length / 2.0))
                                            {
                                                var latitudes = new double[splittedPositionsArray.Length];
                                                var longitudes = new double[splittedPositionsArray.Length];

                                                Parallel.For(0, splittedPositionsArray.Length, index =>
                                                {
                                                    // try to avoid this overload since this one is quite a bit slower than the simple TryParse!
                                                    //if (double.TryParse(splittedPositionArray[index], NumberStyles.Float, new CultureInfo("en-US"), out double positionValue) == true)
                                                    if (double.TryParse(
                                                        splittedPositionsArray[index].Replace(splittedPositionsArray[index].Contains('.') ? "." : ",", currentCulture.NumberFormat.NumberDecimalSeparator),
                                                        out double positionValue) == true)
                                                    {
                                                        if (((BigInteger)index).IsEven)
                                                        {
                                                            latitudes[index] = positionValue;
                                                        }
                                                        else
                                                        {
                                                            longitudes[index] = positionValue;
                                                        }
                                                    }
                                                });

                                                for (int i = 0; i < latitudes.Length; i += 2)
                                                {
                                                    if (InvertLonLat)
                                                    {
                                                        exteriorMapPoints.Add(new MapPoint(longitudes[i + 1], latitudes[i], spatialReferenceSystem));
                                                    }
                                                    else
                                                    {
                                                        exteriorMapPoints.Add(new MapPoint(latitudes[i], longitudes[i + 1], spatialReferenceSystem));
                                                    }
                                                }

                                                longitudes = null;
                                                latitudes = null;
                                            }
                                        }
                                    }

                                    segments.Add(exteriorMapPoints);
                                }
                            }

                            var interiorNode = polygonPatchNode?.SelectSingleNode("gml:interior", mgr);
                            if (interiorNode != null && interiorNode.HasChildNodes)
                            {
                                var interiorMapPoints = new List<MapPoint>();
                                var linearRingNodes = interiorNode.ChildNodes;
                                foreach (XmlNode linearRingNode in linearRingNodes)
                                {
                                    var posListNode = linearRingNode.SelectSingleNode("gml:posList", mgr);
                                    if (posListNode != null)
                                    {
                                        string[] splittedPositionArray =
                                            posListNode.InnerText
                                                .Replace("\t", " ")
                                                .Replace("\n", " ")
                                                .Replace("\r", " ")
                                                .Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                                        if (((double)splittedPositionArray.Length / 2.0) == Math.Abs(splittedPositionArray.Length / 2.0))
                                        {
                                            var latitudes = new double[splittedPositionArray.Length];
                                            var longitudes = new double[splittedPositionArray.Length];

                                            Parallel.For(0, splittedPositionArray.Length, index =>
                                            {
                                                // try to avoid this overload since this one is quite a bit slower than the simple TryParse!
                                                //if (double.TryParse(splittedPositionArray[index], NumberStyles.Float, new CultureInfo("en-US"), out double positionValue) == true)
                                                if (double.TryParse(
                                                    splittedPositionArray[index].Replace(splittedPositionArray[index].Contains('.') ? "." : ",", currentCulture.NumberFormat.NumberDecimalSeparator),
                                                    out double positionValue) == true)
                                                {
                                                    if (((BigInteger)index).IsEven)
                                                    {
                                                        latitudes[index] = positionValue;
                                                    }
                                                    else
                                                    {
                                                        longitudes[index] = positionValue;
                                                    }
                                                }
                                            });

                                            for (int i = 0; i < latitudes.Length; i += 2)
                                            {
                                                if (InvertLonLat)
                                                {
                                                    interiorMapPoints.Add(new MapPoint(longitudes[i + 1], latitudes[i], spatialReferenceSystem));
                                                }
                                                else
                                                {
                                                    interiorMapPoints.Add(new MapPoint(latitudes[i], longitudes[i + 1], spatialReferenceSystem));
                                                }
                                            }

                                            longitudes = null;
                                            latitudes = null;
                                        }
                                    }

                                    segments.Add(interiorMapPoints);
                                }
                            }
                        }
                    }

                    if (segments.Count() > 0)
                    {
                        if (IsPositionInAnySegmentInverted(segments))
                        {
                            segments = InvertPositionsInSegments(segments);
                            InvertLonLat = true;
                        }

                        var polygon = new Polygon(segments, spatialReferenceSystem);
                        return polygon;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
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
