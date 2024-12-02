using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.IO;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class TidalStation : GeoFeatureBase, IS104Feature, ITidalStation
    {
        public Dictionary<string, string> TidalHeights { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> TidalTrends { get; set; } = new Dictionary<string, string>(); 
        public short SelectedIndex { get; set; } 
        public string SelectedDateTime { get; set; } = string.Empty;
        public string SelectedHeight { get; set; } = string.Empty;
        public string SelectedTrend { get; set; } = string.Empty;

        /// <summary>
        ///     
        /// </summary>
        public TidalStation()
        {
            FeatureToolWindow = true; // feature supports tool window
            FeatureToolWindowTemplate = "labelFeatureName=FeatureName.DisplayName,labelProperty1=SelectedDateTime,labelProperty2=SelectedHeight,labelProperty3=SelectedTrend";
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
                    var symbolUri = new Uri($"file:/{Path.Combine(AppContext.BaseDirectory, @"images\TIDEHT01.png")}");
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new TidalStation
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null
                    ? new DateRange()
                    : FixedDateRange.DeepClone() as IDateRange,
                Id = Id,
                PeriodicDateRange = PeriodicDateRange == null
                    ? Array.Empty<DateRange>()
                    : Array.ConvertAll(PeriodicDateRange, p => p.DeepClone() as IDateRange),
                SourceIndication = SourceIndication == null
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                TextContent = TextContent == null
                    ? Array.Empty<TextContent>()
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                TidalHeights = TidalHeights,
                TidalTrends = TidalTrends,
                SelectedIndex = SelectedIndex,
                Links = Links == null
                    ? Array.Empty<Link>()
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr, nameSpacePrefix);

            // implement reading height and trend values from XML!

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override string GetSymbolName()
        {
            return "";
        }

    }
}
