using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;

namespace S1XViewer.Types.Interfaces
{
    public interface IFeatureRendererManager
    {
        List<ColorSchemeRangeItem> ColorScheme { get; set; }
        FeatureCollectionTable Add(string key, FeatureCollectionTable featureCollectionTable);
        void Clear();
        FeatureCollectionTable Create(string key, List<Field> fields, GeometryType geometryType, SpatialReference? spatialReference, bool isVector = false, Renderer? renderer = null);
        FeatureCollectionTable? Get(string key);
        void LoadColorScheme(string fileName, string standard);
        void Remove(string key);
        string[] RetrieveColorSchemeNames();
    }
}
