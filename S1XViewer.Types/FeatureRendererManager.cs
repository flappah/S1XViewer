using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using S1XViewer.Base;
using S1XViewer.Types.Interfaces;
using System.IO;
using System.Xml;

namespace S1XViewer.Types
{
    public class FeatureRendererManager : IFeatureRendererManager
    {
        private static Dictionary<string, FeatureCollectionTable> _featureCollectionTables = new Dictionary<string, FeatureCollectionTable>();
        private object _lockOnThis = new object();

        public List<ColorSchemeRangeItem> ColorScheme { get; set; } = new List<ColorSchemeRangeItem>();

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
        ///     Returns a list of colorschemes
        /// </summary>
        /// <returns></returns>
        public string[] RetrieveColorSchemeNames()
        {
            var fullPath = System.Reflection.Assembly.GetAssembly(GetType())?.Location;
            var directory = Path.GetDirectoryName(fullPath);
            var files = Directory.GetFiles($@"{directory}\colorschemes");
            var flattenedFiles = files.Select(t => t.LastPart(@"\")).ToArray();
            Array.Sort(flattenedFiles);
            return flattenedFiles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="standard"></param>
        public bool LoadColorScheme(string fileName, string standard)
        {
            var fullPath = System.Reflection.Assembly.GetAssembly(GetType())?.Location;
            var directory = Path.GetDirectoryName(fullPath);

            if (File.Exists($@"{directory}\colorschemes\{fileName}") == false)
            {
                return false;            
            }
            
            var xmlDocument = new XmlDocument();
            xmlDocument.Load($@"{directory}\colorschemes\{fileName}");

            XmlNodeList? ranges = xmlDocument.DocumentElement?.SelectNodes($"ColorScheme[@type='{standard}']/Range");
            if (ranges != null && ranges.Count > 0)
            {
                ColorScheme = new List<ColorSchemeRangeItem>();
                foreach (XmlNode range in ranges)
                {
                    var item = new ColorSchemeRangeItem();
                    item.Parse(range);
                    ColorScheme.Add(item);
                }
            }
            else
            {
                // if there are no colorschemes add one black value
                ColorScheme.Add(new ColorSchemeRangeItem { Color = System.Windows.Media.Colors.Black, Max = 15000.0, Min = -15000.0 });
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="featureCollectionTable"></param>
        /// <returns></returns>
        public FeatureCollectionTable Add(string key, FeatureCollectionTable featureCollectionTable)
        {
            lock (_lockOnThis)
            {
                if (_featureCollectionTables.ContainsKey(key) == false)
                {
                    _featureCollectionTables.Add(key, featureCollectionTable);
                }

                return featureCollectionTable;
            }
        }

        /// <summary>
        ///     Clears the internal storage
        /// </summary>
        public void Clear()
        {
            foreach(KeyValuePair<string, FeatureCollectionTable> table in _featureCollectionTables)
            {
                table.Value.CancelLoad();
            }

            _featureCollectionTables.Clear();
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
            lock (_lockOnThis)
            {
                if (_featureCollectionTables.ContainsKey(key) == true)
                {
                    return _featureCollectionTables[key];
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns all featurecollectiontable-s
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, FeatureCollectionTable> Getall()
        {
            return _featureCollectionTables;
        }

        /// <summary>
        ///     Removes the stored FeatureCollectionTable
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) 
        {
            lock (_lockOnThis)
            {
                if (_featureCollectionTables[key] != null)
                {
                    _featureCollectionTables.Remove(key);
                }
            }
        }
    }
}
