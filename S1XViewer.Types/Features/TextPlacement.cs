using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class TextPlacement : GeoFeatureBase, ITextPlacement, IS122Feature, IS123Feature, IS127Feature
    {
        public string FlipBearing { get; set; } = string.Empty;
        public string ScaleMinimum { get; set; } = string.Empty;
        public string TextJustification { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string TextType { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new TextPlacement
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
                FlipBearing = FlipBearing,
                ScaleMinimum = ScaleMinimum,
                TextJustification = TextJustification,
                Text = Text,
                TextType = TextType,
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
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr);

            var flipBearingNode = node.SelectSingleNode("flipBearing", mgr);
            if (flipBearingNode != null && flipBearingNode.HasChildNodes)
            {
                FlipBearing = flipBearingNode.FirstChild?.InnerText ?? string.Empty;
            }

            var scaleMinimumNode = node.SelectSingleNode("scaleMinimum", mgr);
            if (scaleMinimumNode != null && scaleMinimumNode.HasChildNodes)
            {
                ScaleMinimum = scaleMinimumNode.FirstChild?.InnerText ?? string.Empty;
            }

            var textJustificationNode = node.SelectSingleNode("textJustification", mgr);
            if (textJustificationNode != null && textJustificationNode.HasChildNodes)
            {
                TextJustification = textJustificationNode.FirstChild?.InnerText ?? string.Empty;
            }

            var textNode = node.SelectSingleNode("text", mgr);
            if (textNode != null && textNode.HasChildNodes)
            {
                Text = textNode.FirstChild?.InnerText ?? string.Empty;
            }

            var textTypeNode = node.SelectSingleNode("textType", mgr);
            if (textTypeNode != null && textTypeNode.HasChildNodes)
            {
                TextType = textTypeNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            return "";
        }
    }
}
