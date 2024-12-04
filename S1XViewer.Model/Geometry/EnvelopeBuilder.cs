using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class EnvelopeBuilder : GeometryBuilderBase, IEnvelopeBuilder
    {
        /// <summary>
        ///     For injection purposes
        /// </summary>
        public EnvelopeBuilder()
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

            Envelope createdEnvelope;
            if (InvertLonLat)
            {
                createdEnvelope =
                    new Envelope(y[0], x[0], y[1], x[1], SpatialReference.Create(_spatialReferenceSystem));
            }
            else
            {
                createdEnvelope =
                    new Envelope(x[0], y[0], x[1], y[1], SpatialReference.Create(_spatialReferenceSystem));
            }
            return createdEnvelope;
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

            if (node.ChildNodes[0].ChildNodes.Count == 2)
            {
                var lowerLeft = node.ChildNodes[0].ChildNodes[0].InnerText;
                var upperRight = node.ChildNodes[0].ChildNodes[1].InnerText;

                var llPos = lowerLeft.Replace(@"\t", " ").Replace(@"\n", " ").Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                var urPos = upperRight.Replace(@"\t", " ").Replace(@"\n", " ").Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                if (!double.TryParse(llPos[0], NumberStyles.Float, new CultureInfo("en-US"), out double llX))
                {
                    llX = 0.0;
                }
                if (!double.TryParse(llPos[1], NumberStyles.Float, new CultureInfo("en-US"), out double llY))
                {
                    llY = 0.0;
                }

                if (!double.TryParse(urPos[0], NumberStyles.Float, new CultureInfo("en-US"), out double urX))
                {
                    urX = 0.0;
                }
                if (!double.TryParse(urPos[1], NumberStyles.Float, new CultureInfo("en-US"), out double urY))
                {
                    urY = 0.0;
                }

                Esri.ArcGISRuntime.Geometry.Envelope createdEnvelope;
                if (InvertLonLat == false)
                {
                    if (llY < -90.0 || llY > 90.0 || urY < -90.0 || urY > 90.0)
                    {
                        createdEnvelope =
                            new Esri.ArcGISRuntime.Geometry.Envelope(llY, llX, urY, urX, SpatialReference.Create(_spatialReferenceSystem));

                        InvertLonLat = true;
                    }
                    else
                    {
                        createdEnvelope =
                            new Esri.ArcGISRuntime.Geometry.Envelope(llX, llY, urX, urY, SpatialReference.Create(_spatialReferenceSystem));
                    }
                }
                else
                {
                    createdEnvelope =
                        new Esri.ArcGISRuntime.Geometry.Envelope(llY, llX, urY, urX, SpatialReference.Create(_spatialReferenceSystem));
                }

                return createdEnvelope;
            }

            return null;
        }
    }
}
