using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public abstract class GeoFeatureBase : FeatureBase, IGeoFeature
    {
        public IFeatureName[] FeatureName { get; set; }
        public IDateRange FixedDateRange { get; set; }
        public IDateRange[] PeriodicDateRange { get; set; }
        public ISourceIndication SourceIndication { get; set; }
        public ITextContent[] TextContent { get; set; }

        public Geometry Geometry { get; set; }
    }
}
