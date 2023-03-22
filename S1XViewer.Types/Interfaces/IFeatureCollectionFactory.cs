using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;

namespace S1XViewer.Types.Interfaces
{
    public interface IFeatureCollectionFactory
    {
        FeatureCollectionTable Add(string key, FeatureCollectionTable featureCollectionTable);
        FeatureCollectionTable Create(string key, List<Field> fields, GeometryType geometryType, SpatialReference? spatialReference, bool isVector = false, Renderer? renderer = null);
        FeatureCollectionTable? Get(string key);
        void Remove(string key);
    }
}
