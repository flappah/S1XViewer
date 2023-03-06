using S1XViewer.Types.Interfaces;
using System.Xml;
using S1XViewer.Types;
using Esri.ArcGISRuntime.Geometry;

namespace S1XViewer.Types
{
    public class S1xxDataPackage : IS1xxDataPackage
    {
        public S1xxTypes Type { get; set; }
        public IMetaFeature[] MetaFeatures { get; set; }
        public IInformationFeature[] InformationFeatures { get; set; }
        public IGeoFeature[] GeoFeatures { get; set; }
        public Geometry BoundingBox { get; set; }
        public XmlDocument RawXmlData { get; set; }
    }
}
