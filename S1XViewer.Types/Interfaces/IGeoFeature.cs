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

        Geometry Geometry { get; set; }

        (Feature?, Esri.ArcGISRuntime.UI.Graphic?) Render(FeatureCollectionTable featureTable);
        (Dictionary<System.Drawing.Color, FeatureCollectionTable>, Feature?) GetFeature(List<Field> fields, Dictionary<System.Drawing.Color, FeatureCollectionTable> featureTableCollection, SpatialReference? horizontalCRS);
    }
}