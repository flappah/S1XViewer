using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Drawing;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Sounding : GeoFeatureBase, ISounding, IS102Feature
    {
        public float Value { get; set; } = 0.0f;
        public string[] QualityOfVerticalMeasurement { get; set; } = Array.Empty<string>(); 
        public string Status { get; set; } = string.Empty; 
        public string[] TechniqueOfVerticalMeasurement { get; set; } = Array.Empty<string>();
        public IVerticalUncertainty? VerticalUncertainty { get; set; } = new VerticalUncertainty();
        
        /// <summary>
        ///     Renders the feature for use in ArcGIS Runtime
        /// </summary>
        /// <param name="featureTable"></param>
        /// <returns></returns>
        public override (string type, Feature? feature, Esri.ArcGISRuntime.UI.Graphic? graphic) Render(IFeatureRendererManager featureRendererManager, SpatialReference? horizontalCRS)
        {
            Field idField = new Field(FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(FieldType.Text, "FeatureName", "Name", 255);

            if (Geometry is Polygon polygon)
            {
                var color = Color.Black;
                foreach (ColorRampItem schemeItem in featureRendererManager.ColorRamp)
                {
                    if (schemeItem.Between(Value))
                    {
                        color = System.Drawing.Color.FromArgb(schemeItem.Color.A, schemeItem.Color.R, schemeItem.Color.G, schemeItem.Color.B);
                        break;
                    }
                }

                var lineSym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, color, 1);
                var sym = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color, lineSym);
                var simpleRenderer = new SimpleRenderer(sym);

                var key = $"FilledPolyFeatures_{color.R.ToString()}{color.G.ToString()}{color.B.ToString()}";
                FeatureCollectionTable? featureCollectionTable = featureRendererManager.Get(key);
                if (featureCollectionTable == null)
                {
                    featureCollectionTable = featureRendererManager.Create(key, new List<Field> { idField, nameField }, GeometryType.Polygon, horizontalCRS, false, simpleRenderer);
                }

                if (featureCollectionTable != null)
                {
                    Feature polyFeature = featureCollectionTable.CreateFeature();
                    polyFeature.SetAttributeValue(idField, Id);
                    polyFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                    polyFeature.Geometry = Geometry;

                    return (key, polyFeature, null);
                }
            }

            return base.Render(featureRendererManager, horizontalCRS);
        }

        /// <summary>
        ///     Deepclones the feature
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IFeature DeepClone()
        {
            return new Sounding
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null
                    ? new DateRange()
                    : FixedDateRange.DeepClone() as IDateRange,
                Id = Id ?? "",
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
                QualityOfVerticalMeasurement = QualityOfVerticalMeasurement == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(QualityOfVerticalMeasurement, s => s),
                ReportedDate = ReportedDate,
                Status= Status,
                TechniqueOfVerticalMeasurement = TechniqueOfVerticalMeasurement == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(TechniqueOfVerticalMeasurement, s => s),
                VerticalUncertainty = VerticalUncertainty == null 
                    ? new VerticalUncertainty()
                    : VerticalUncertainty.DeepClone() as IVerticalUncertainty,
                Value = Value,
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

            var valueNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}value", mgr);
            if (valueNode != null && valueNode.HasChildNodes)
            {
                if (float.TryParse(valueNode.FirstChild?.InnerText, out float valueValue))
                {
                    Value = valueValue;
                }
            }

            var qualityOfVerticalMeasurementNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}qualityOfVerticalMeasurement", mgr);
            if (qualityOfVerticalMeasurementNodes != null && qualityOfVerticalMeasurementNodes.Count > 0)
            {
                var measurements = new List<string>();
                foreach (XmlNode qualityOfVerticalMeasurementNode in qualityOfVerticalMeasurementNodes)
                {
                    if (qualityOfVerticalMeasurementNode != null && qualityOfVerticalMeasurementNode.HasChildNodes)
                    {
                        var measurement = qualityOfVerticalMeasurementNode.FirstChild?.InnerText ?? string.Empty;
                        measurements.Add(measurement);
                    }
                }

                measurements.Sort();
                QualityOfVerticalMeasurement = measurements.ToArray();
            }

            var statusNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}status", mgr);
            if (statusNode != null && statusNode.HasChildNodes)
            {
                Status = statusNode.FirstChild?.InnerText ?? string.Empty;
            }

            var techniqueOfVerticalMeasurementNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}techniqueOfVerticalMeasurement", mgr);
            if (techniqueOfVerticalMeasurementNodes != null && techniqueOfVerticalMeasurementNodes.Count > 0)
            {
                var measurements = new List<string>();
                foreach (XmlNode techniqueOfVerticalMeasurementNode in techniqueOfVerticalMeasurementNodes)
                {
                    if (techniqueOfVerticalMeasurementNode != null && techniqueOfVerticalMeasurementNode.HasChildNodes)
                    {
                        var measurement = techniqueOfVerticalMeasurementNode.FirstChild?.InnerText ?? string.Empty;
                        measurements.Add(measurement);
                    }
                }

                measurements.Sort();
                TechniqueOfVerticalMeasurement = measurements.ToArray();
            }

            var verticalUncertaintyNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}verticalUncertainty", mgr);
            if (verticalUncertaintyNode != null && verticalUncertaintyNode.HasChildNodes)
            {
                VerticalUncertainty = new VerticalUncertainty();
                VerticalUncertainty.FromXml(node, mgr, nameSpacePrefix);
            }

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
