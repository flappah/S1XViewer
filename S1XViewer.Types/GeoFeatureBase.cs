using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public abstract class GeoFeatureBase : FeatureBase, IGeoFeature
    {
        public IFeatureName[] FeatureName { get; set; }
        public IDateRange FixedDateRange { get; set; }
        public IDateRange[] PeriodicDateRange { get; set; }
        public ISourceIndication SourceIndication { get; set; }
        public ITextContent[] TextContent { get; set; }

        public Geometry Geometry { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="distance"></param>
        /// <param name="bearing"></param>
        /// <returns></returns>
        protected (double Lat, double Lon) Destination((double Lat, double Lon) startPoint, double distance, double bearing)
        {
            var radius = 6378001;
            double lat1 = startPoint.Lat * (Math.PI / 180);
            double lon1 = startPoint.Lon * (Math.PI / 180);
            double brng = bearing * (Math.PI / 180);
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(distance / radius) + Math.Cos(lat1) * Math.Sin(distance / radius) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(distance / radius) * Math.Cos(lat1), Math.Cos(distance / radius) - Math.Sin(lat1) * Math.Sin(lat2));
            return (lat2 * (180 / Math.PI), lon2 * (180 / Math.PI));
        }

        /// <summary>
        ///     Creates the renderer for features on the map
        /// </summary>
        /// <param name="rendererType"></param>
        /// <returns></returns>
        protected Renderer CreateRenderer(GeometryType rendererType, bool isVector = false)
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
        ///     Creates an ARCGIS feature. Base contains no implemenation!
        /// </summary>
        /// <param name="featureTableCollection"></param>
        /// <returns></returns>
        public virtual (Dictionary<System.Drawing.Color, FeatureCollectionTable>, Feature?) GetFeature(List<Field> fields, Dictionary<System.Drawing.Color, FeatureCollectionTable> featureTableCollection, SpatialReference? horizontalCRS)
        {
            return (new Dictionary<System.Drawing.Color, FeatureCollectionTable>(), null);
        }

        /// <summary>
        ///     Creates an ARCGIS feature
        /// </summary>
        /// <returns></returns>
        public virtual (Feature?, Esri.ArcGISRuntime.UI.Graphic?) Render(FeatureCollectionTable featureTable)
        {
            Field idField = new Field(FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(FieldType.Text, "FeatureName", "Name", 255);

            if (Geometry is MapPoint mapPoint)
            {
                Feature pointFeature = featureTable.CreateFeature();
                pointFeature.SetAttributeValue(idField, Id);
                pointFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                pointFeature.Geometry = Geometry;

                return (pointFeature, null);
            }
            else if (Geometry is Polyline)
            {
                Feature lineFeature = featureTable.CreateFeature();
                lineFeature.SetAttributeValue(idField, Id);
                lineFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                lineFeature.Geometry = Geometry;

                return (lineFeature, null);
            }
            else
            {
                Feature polyFeature = featureTable.CreateFeature();
                polyFeature.SetAttributeValue(idField, Id);
                polyFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                polyFeature.Geometry = Geometry;

                return (polyFeature, null);
            }
        }
    }
}
