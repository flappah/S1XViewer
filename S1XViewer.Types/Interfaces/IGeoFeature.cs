using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;

namespace S1XViewer.Types.Interfaces
{
    public interface IGeoFeature : IFeature
    {
        IFeatureName[] FeatureName { get; set; }
        IDateRange FixedDateRange { get; set; }
        IDateRange[] PeriodicDateRange { get; set; }
        ISourceIndication SourceIndication { get; set; }
        ITextContent[] TextContent { get; set; }

        Esri.ArcGISRuntime.Geometry.Geometry? Geometry { get; set; }

        string GetSymbolName();

        (string type, Feature? feature, Esri.ArcGISRuntime.UI.Graphic? graphic) Render(IFeatureRendererManager featureRendererManager, SpatialReference? horizontalCRS);
    }
}