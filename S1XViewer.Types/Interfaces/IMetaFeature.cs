using Esri.ArcGISRuntime.Geometry;

namespace S1XViewer.Types.Interfaces
{
    public interface IMetaFeature : IFeature
    {
        Geometry? Geometry { get; set; }
    }
}
