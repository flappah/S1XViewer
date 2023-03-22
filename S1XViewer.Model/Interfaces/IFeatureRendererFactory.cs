using Esri.ArcGISRuntime.Data;

namespace S1XViewer.Model.Interfaces
{
    public interface IFeatureRendererFactory
    {
        FeatureCollectionTable Add(string key, FeatureCollectionTable featureCollectionTable);
        FeatureCollectionTable Get(string key);
        void Remove(string key);
    }
}
