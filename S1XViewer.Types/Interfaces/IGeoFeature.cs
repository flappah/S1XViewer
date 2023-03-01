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

        Geometry Geometry { get; set; }
    }
}