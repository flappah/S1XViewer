using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class PointBuilder : GeometryBuilderBase, IPointBuilder
    {
        /// <summary>
        ///     For injection purposes
        /// </summary>
        /// <param name="optionsStorage"></param>
        public PointBuilder(IOptionsStorage optionsStorage)
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

                    if (invertLatLon)
                    {
                        return new MapPoint(y, x, SpatialReference.Create(_spatialReferenceSystem));
                    }
                    else
                    {
                        return new MapPoint(x, y, SpatialReference.Create(_spatialReferenceSystem));
                    }
                }
            }

            return null;
        }
    }
}
