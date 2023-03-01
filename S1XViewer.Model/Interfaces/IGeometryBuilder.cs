using System.Xml;

namespace S1XViewer.Model.Interfaces
{
    public interface IGeometryBuilder
    {
        Esri.ArcGISRuntime.Geometry.Geometry FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}
