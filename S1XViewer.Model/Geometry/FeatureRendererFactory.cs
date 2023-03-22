using Esri.ArcGISRuntime.Data;
using S1XViewer.Model.Interfaces;

namespace S1XViewer.Model.Geometry
{
    public class FeatureRendererFactory : IFeatureRendererFactory
    {
        private static Dictionary<string, FeatureCollectionTable> _featureCollectionTables = new Dictionary<string, FeatureCollectionTable>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="featureCollectionTable"></param>
        /// <returns></returns>
        public FeatureCollectionTable Add(string key, FeatureCollectionTable featureCollectionTable)
        {
            if (_featureCollectionTables.ContainsKey(key) == false)
            {
                _featureCollectionTables.Add(key, featureCollectionTable);
            }

            return featureCollectionTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public FeatureCollectionTable Get(string key)
        {
            if (_featureCollectionTables[key] != null)
            {
                return _featureCollectionTables[key];   
            }

            throw new Exception($"FeatureCollectionTable '{key}' does not exist!");
        }

        /// <summary>
        ///     Removes the stored FeatureCollectionTable
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) 
        {
            if (_featureCollectionTables[key] != null)
            {
                _featureCollectionTables.Remove(key);
            }
        }
    }
}
