using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Authority : InformationFeatureBase, IAuthority, IS122Feature, IS123Feature, IS127Feature
    {
        public string CategoryOfAuthority { get; set; } = string.Empty;
        public ITextContent TextContent { get; set; } = new TextContent();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new Authority
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
                    ? Array.Empty<SourceIndication>()
                    : Array.ConvertAll(SourceIndication, s => s.DeepClone() as ISourceIndication),
                CategoryOfAuthority = CategoryOfAuthority ?? "",
                TextContent = TextContent == null ? null : TextContent.DeepClone() as ITextContent,
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

            var categoryOfAuthorityNode = node.SelectSingleNode("categoryOfAuthority", mgr);
            if (categoryOfAuthorityNode != null && categoryOfAuthorityNode.HasChildNodes)
            {
                CategoryOfAuthority = categoryOfAuthorityNode.FirstChild?.InnerText ?? string.Empty;
            }

            var textContentNode = node.SelectSingleNode("textContent", mgr);
            if (textContentNode != null && textContentNode.HasChildNodes)
            {
                TextContent = new TextContent();
                TextContent.FromXml(textContentNode, mgr);
            }

            return this;
        }
    }
}
