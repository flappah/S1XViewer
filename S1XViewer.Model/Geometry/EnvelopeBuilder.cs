using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class EnvelopeBuilder : GeometryBuilderBase, IEnvelopeBuilder
    {
        /// <summary>
        ///     For injection purposes
        /// </summary>
        /// <param name="optionsStorage"></param>
        public EnvelopeBuilder(IOptionsStorage optionsStorage)
        {
            _optionsStorage = optionsStorage;
        }

        /// <summary>
        ///     Retrieves the geometry from the specified Xml Node
        /// </summary>
        /// <param name="node">node containing a basic geometry</param>
        /// <param name="mgr">namespace manager</param>
        /// <returns>ESRI Arc GIS geometry</returns>
        public override Esri.ArcGISRuntime.Geometry.Geometry FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node != null && node.HasChildNodes)
            {
                XmlNode srsNode = null;
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
                    string defaultCRS = _optionsStorage.Retrieve("comboBoxCRS");
                    if (int.TryParse(defaultCRS, out int defaultCRSValue))
                    {
                        _spatialReferenceSystem = defaultCRSValue; // if no srsNode is found assume default reference systema
                    }
                    else
                    {
                        _spatialReferenceSystem = 4326; // since most S1xx standards assume WGS84 is default, use this is the uber default CRS
                    }
                }

                string invertLatLonString = _optionsStorage.Retrieve("checkBoxInvertLatLon");
                if (!bool.TryParse(invertLatLonString, out bool invertLatLon))
                {
                    invertLatLon = false;
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

                    if (invertLatLon)
                    {
                        createdEnvelope =
                            new Esri.ArcGISRuntime.Geometry.Envelope(llY, llX, urY, urX, SpatialReference.Create(_spatialReferenceSystem));
                    }
                    else
                    {
                        createdEnvelope =
                            new Esri.ArcGISRuntime.Geometry.Envelope(llX, llY, urX, urY, SpatialReference.Create(_spatialReferenceSystem));
                    }

                    return createdEnvelope;
                }
            }

            return null;
        }
    }
}
