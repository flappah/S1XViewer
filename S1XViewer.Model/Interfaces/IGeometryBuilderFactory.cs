using System.Xml;

namespace S1XViewer.Model.Interfaces
{
    public interface IGeometryBuilderFactory
    {
        IGeometryBuilder[] Builders { get; set; }
        bool InvertLatLon { get; set; }
        string DefaultCRS { get; set; }

        Esri.ArcGISRuntime.Geometry.Geometry? Create(XmlNode node, XmlNamespaceManager mgr);
        Esri.ArcGISRuntime.Geometry.Geometry? Create(string geometryTypeString, double[] x, double[] y, int srs = -1);
        Esri.ArcGISRuntime.Geometry.Geometry? Create(string geometryTypeString, double[] x, double[] y, double z, int srs = -1);
    }
}