using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using S1XViewer.Types.Interfaces;
using System.IO;

namespace S1XViewer.Types.Features
{
    public abstract class Layout : SupervisedArea, ILayout, IS131Feature
    {
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
                if (mapPoint != null && File.Exists(Path.Combine(AppContext.BaseDirectory, @$"images\{GetSymbolName()}.png")))
                {
                    var symbolUri = new Uri($"file:/{Path.Combine(AppContext.BaseDirectory, @$"images\{GetSymbolName()}.png")}");
                    var symbol = new PictureMarkerSymbol(symbolUri) { Width = 13, Height = 13 };

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

