using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public abstract class MetaFeatureBase : FeatureBase, IMetaFeature
    {
        public Geometry Geometry { get; set; }
    }
}
