using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Symbology;

namespace S1XViewer.Types.Interfaces
{
    public interface IFeatureRendererManager
    {
        List<ColorRampItem> ColorRamp { get; set; }
        FeatureCollectionTable Add(string key, FeatureCollectionTable featureCollectionTable);
        void Clear();
        FeatureCollectionTable Create(string key, List<Field> fields, GeometryType geometryType, SpatialReference? spatialReference, bool isVector = false, Renderer? renderer = null);
        FeatureCollectionTable? Get(string key);
        Dictionary<string, FeatureCollectionTable> Getall();
        Colormap? GetColormap(string fileName, string standard);
        bool LoadColorRamp(string fileName, string standard);
        void Remove(string key);
        string[] RetrieveColorRampNames();
    }
}
