using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
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
        ///     Renders an ARCGIS feature
        /// </summary>
        /// <param name="featureRendererManager"></param>
        /// <returns></returns>
        public virtual (string type, Feature feature, Esri.ArcGISRuntime.UI.Graphic? graphic) Render(IFeatureRendererManager featureRendererManager, SpatialReference? horizontalCRS)
        {
            Field idField = new Field(FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(FieldType.Text, "FeatureName", "Name", 255);

            if (Geometry is MapPoint mapPoint)
            {
                FeatureCollectionTable featureTable = featureRendererManager.Get("PointFeatures");

                Feature pointFeature = featureTable.CreateFeature();
                pointFeature.SetAttributeValue(idField, Id);
                pointFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                pointFeature.Geometry = Geometry;

                return ("PointFeatures", pointFeature, null);
            }
            else if (Geometry is Polyline)
            {
                FeatureCollectionTable featureTable = featureRendererManager.Get("LineFeatures");

                Feature lineFeature = featureTable.CreateFeature();
                lineFeature.SetAttributeValue(idField, Id);
                lineFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                lineFeature.Geometry = Geometry;

                return ("LineFeatures", lineFeature, null);
            }
            else
            {
                FeatureCollectionTable featureTable = featureRendererManager.Get("PolygonFeatures");

                Feature polyFeature = featureTable.CreateFeature();
                polyFeature.SetAttributeValue(idField, Id);
                polyFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                polyFeature.Geometry = Geometry;

                return ("PolygonFeatures", polyFeature, null);
            }
        }
    }
}
