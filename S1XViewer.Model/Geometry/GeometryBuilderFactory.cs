using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using System;
using System.Linq;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class GeometryBuilderFactory : IGeometryBuilderFactory
    {
        public IGeometryBuilder[] Builders { get; set; }

        public Esri.ArcGISRuntime.Geometry.Geometry Create(XmlNode node, XmlNamespaceManager mgr)
        {
            string geometryTypeString;

            if (!node.Name.Contains(false, "Envelope", "Curve", "Point", "Polygon", "Surface"))
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
    }
}
