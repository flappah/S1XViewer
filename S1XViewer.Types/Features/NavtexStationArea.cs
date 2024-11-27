using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class NavtexStationArea : GeoFeatureBase, INavtexStationArea, IS123Feature
    {
        public string TxIdentChar { get; set; } = string.Empty;
        public string Restriction { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new NavtexStationArea
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
                TxIdentChar = TxIdentChar,
                Restriction = Restriction,
                Status = Status,
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

            var txIdentCharNode = node.SelectSingleNode("txIdentChar", mgr);
            if (txIdentCharNode != null && txIdentCharNode.HasChildNodes)
            {
                TxIdentChar = txIdentCharNode.FirstChild.InnerText;
            }

            var restrictionNode = node.SelectSingleNode("restriction", mgr);
            if (restrictionNode != null && restrictionNode.HasChildNodes)
            {
                Restriction = restrictionNode.FirstChild.InnerText;
            }

            var statusNode = node.SelectSingleNode("status", mgr);
            if (statusNode != null && statusNode.HasChildNodes)
            {
                Status = statusNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
