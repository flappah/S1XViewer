using System.Xml;

namespace S1XViewer.Model.Interfaces
{
    public interface IGeometryBuilderFactory
    {
        IGeometryBuilder[] Builders { get; set; }
        Esri.ArcGISRuntime.Geometry.Geometry Create(XmlNode node, XmlNamespaceManager mgr);
    }
}