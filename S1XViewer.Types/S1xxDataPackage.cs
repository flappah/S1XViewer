using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S1xxDataPackage : IS1xxDataPackage
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public S1xxTypes Type { get; set; }
        public IMetaFeature[] MetaFeatures { get; set; }
        public IInformationFeature[] InformationFeatures { get; set; }
        public IGeoFeature[] GeoFeatures { get; set; }
        public Geometry BoundingBox { get; set; }
    }
}
