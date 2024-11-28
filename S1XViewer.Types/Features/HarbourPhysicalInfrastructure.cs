using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public abstract class HarbourPhysicalInfrastructure : SupervisedArea, IHarbourPhysicalInfrastructure, IS131Feature
    {
        public float VerticalClearance { get; set; } = 0.0f;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            base.FromXml(node, mgr);

            var verticalClearanceValueNode = node.SelectSingleNode("verticalClearanceValue", mgr);
            if (verticalClearanceValueNode != null && verticalClearanceValueNode.HasChildNodes)
            {
                if (float.TryParse(verticalClearanceValueNode.FirstChild?.InnerText, out float verticalClearanceValue))
                {
                    VerticalClearance = verticalClearanceValue;
                }
            }

            return this;
        }

        /// <summary>
        ///     Renders an ARCGIS feature
        /// </summary>
        /// <param name="featureRendererManager"></param>
        /// <returns></returns>
        public override (string type, Feature? feature, Esri.ArcGISRuntime.UI.Graphic? graphic) Render(IFeatureRendererManager featureRendererManager, SpatialReference? horizontalCRS)
        {
            if (featureRendererManager is null)
            {
                throw new ArgumentNullException(nameof(featureRendererManager));
            }

            Field idField = new Field(FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(FieldType.Text, "FeatureName", "Name", 255);

            if (Geometry is MapPoint mapPoint)
            {
                if (mapPoint != null)
                {
                    var symbolUri = new Uri($"file:/{Path.Combine(AppContext.BaseDirectory, @$"images\{GetSymbolName()}.png")}");
                    var symbol = new PictureMarkerSymbol(symbolUri) { Width = 34, Height = 13 };

                    var graphic = new Esri.ArcGISRuntime.UI.Graphic();
                    graphic.Geometry = mapPoint;
                    graphic.Symbol = symbol;

                    FeatureCollectionTable? featureTable = featureRendererManager.Get("VectorFeatures");
                    if (featureTable != null)
                    {
                        Feature pointFeature = featureTable.CreateFeature();
                        pointFeature.SetAttributeValue(idField, Id);
                        pointFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                        pointFeature.Geometry = Geometry;

                        return ("VectorFeatures", pointFeature, graphic);
                    }
                }
            }

            return base.Render(featureRendererManager, horizontalCRS);
        }
    }
}
