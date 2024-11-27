using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class UnderkeelClearanceAllowanceArea : GeoFeatureBase, IUnderkeelClearanceAllowanceArea, IS127Feature
    {
        public IUnderkeelAllowance UnderkeelAllowance { get; set; } = new UnderkeelAllowance();
        public string WaterLevelTrend { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new UnderkeelClearanceAllowanceArea
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
                UnderkeelAllowance = UnderkeelAllowance == null
                    ? new UnderkeelAllowance()
                    : UnderkeelAllowance.DeepClone() as IUnderkeelAllowance,
                WaterLevelTrend = WaterLevelTrend,
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

            var underkeelAllowanceNode = node.SelectSingleNode("underkeelAllowance", mgr);
            if (underkeelAllowanceNode != null && underkeelAllowanceNode.HasChildNodes)
            {
                UnderkeelAllowance = new UnderkeelAllowance();
                UnderkeelAllowance.FromXml(underkeelAllowanceNode, mgr);
            }

            var waterLevelTrendNode = node.SelectSingleNode("waterLevelTrend", mgr);
            if (waterLevelTrendNode != null && waterLevelTrendNode.HasChildNodes)
            {
                WaterLevelTrend = waterLevelTrendNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
