using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class GeometryBuilderFactory : IGeometryBuilderFactory
    {
        public IGeometryBuilder[] Builders { get; set; }

        /// <summary>
        ///     Creates an ESRI geometry based on the provided XML
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public Esri.ArcGISRuntime.Geometry.Geometry Create(XmlNode node, XmlNamespaceManager mgr)
        {
            string geometryTypeString;

            if (node.Name.Contains(false, "Envelope", "Curve", "Point", "Polygon", "Surface") == false)
            {
                geometryTypeString = (node.HasChildNodes ? node.ChildNodes[0].Name : "").LastPart(char.Parse(":"));
            }
            else
            {
                geometryTypeString = node.Name.LastPart(char.Parse(":"));
            }

            var locatedBuilder =
                Builders.ToList().Find(tp => tp.GetType().ToString().Contains($"{geometryTypeString}Builder"));

            if (locatedBuilder != null)
            {
                return locatedBuilder.FromXml(node, mgr);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryTypeString"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Esri.ArcGISRuntime.Geometry.Geometry Create(string geometryTypeString, double[] x, double[] y)
        {
            var locatedBuilder =
                Builders.ToList().Find(tp => tp.GetType().ToString().Contains($"{geometryTypeString}Builder"));

            if (locatedBuilder != null)
            {
                return locatedBuilder.FromPositions(x, y);
            }

            return null;
        }
    }
}
