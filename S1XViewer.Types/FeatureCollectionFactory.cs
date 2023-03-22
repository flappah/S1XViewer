using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class FeatureCollectionFactory : IFeatureCollectionFactory
    {
        private static Dictionary<string, FeatureCollectionTable> _featureCollectionTables = new Dictionary<string, FeatureCollectionTable>();
        private object _lockOnThis = new object();

        /// <summary>
        ///     Creates the renderer for features on the map
        /// </summary>
        /// <param name="rendererType"></param>
        /// <returns></returns>
        private Renderer CreateRenderer(GeometryType rendererType, bool isVector = false)
        {
            // Return a simple renderer to match the geometry type provided
            Symbol sym = null;

            switch (rendererType)
            {
                case GeometryType.Point:
                case GeometryType.Multipoint:
                    // Create a marker symbol
                    if (isVector == true)
                    {
                        sym = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Black, 4);
                    }
                    else
                    {
                        sym = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.X, System.Drawing.Color.Red, 10);
                    }

                    break;

                case GeometryType.Polyline:
                    // Create a line symbol
                    sym = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.DarkGray, 3);
                    break;

                case GeometryType.Polygon:
                    // Create a fill symbol
                    var lineSym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 50, 50, 50), 1);
                    sym = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(25, System.Drawing.Color.LightGray), lineSym);
                    break;

                default:
                    break;
            }

            // Return a new renderer that uses the symbol created above
            return new SimpleRenderer(sym);
        }

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
        /// <param name="fields"></param>
        /// <param name="geometryType"></param>
        /// <param name="spatialReference"></param>
        /// <param name="isVector"></param>
        /// <param name="renderer"></param>
        /// <returns></returns>
        public FeatureCollectionTable Create(string key, List<Field> fields, GeometryType geometryType, SpatialReference? spatialReference, bool isVector = false, Renderer? renderer = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            if (fields is null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            if (_featureCollectionTables.ContainsKey(key) == true)
            {
                return _featureCollectionTables[key];
            }

            if (renderer == null)
            {
                renderer = CreateRenderer(geometryType, isVector);
            }

            var featureCollectionTable = new FeatureCollectionTable(fields, geometryType, spatialReference)
            {
                Renderer = renderer,
                DisplayName = key
            };

            lock (_lockOnThis)
            {
                if (_featureCollectionTables.ContainsKey(key))
                {
                    return _featureCollectionTables[key];
                }

                _featureCollectionTables.Add(key, featureCollectionTable);
                return featureCollectionTable;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public FeatureCollectionTable? Get(string key)
        {
            if (_featureCollectionTables.ContainsKey(key))
            {
                return _featureCollectionTables[key];   
            }

            return null;
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
