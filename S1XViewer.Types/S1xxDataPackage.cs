using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S1xxDataPackage : IS1xxDataPackage
    {
        public Guid Id { get; set; }
        public bool InvertLonLat { get; set; } = false;
        public int DefaultCRS { get; set; } = 4326;
        public string FileName { get; set; } = string.Empty;
        public S1xxTypes Type { get; set; }
        public IMetaFeature[] MetaFeatures { get; set; } = Array.Empty<IMetaFeature>();
        public IInformationFeature[] InformationFeatures { get; set; } = Array.Empty<IInformationFeature>();
        public IGeoFeature[] GeoFeatures { get; set; } = Array.Empty<IGeoFeature>();
        public Geometry? BoundingBox { get; set; }
    }
}
