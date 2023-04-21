using S1XViewer.Model.Interfaces;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public abstract class GeometryBuilderBase : IGeometryBuilder
    {
        protected static int _spatialReferenceSystem;

        public bool InvertLonLat { get; set; } = false;
        public string DefaultCRS { get; set; } = string.Empty;

        public abstract Esri.ArcGISRuntime.Geometry.Geometry? FromXml(XmlNode node, XmlNamespaceManager mgr);
        public abstract Esri.ArcGISRuntime.Geometry.Geometry FromPositions(double[] x, double[] y, int srs = -1);
        public abstract Esri.ArcGISRuntime.Geometry.Geometry FromPositions(double[] x, double[] y, double z, int srs = -1);
    }
}
