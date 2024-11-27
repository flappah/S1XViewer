using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Regulations : AbstractRxn, IRegulations, IS122Feature, IS123Feature, IS127Feature
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new Regulations
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
                Graphic = Graphic == null
                    ? Array.Empty<Graphic>()
                    : Array.ConvertAll(Graphic, g => g.DeepClone() as IGraphic),
                RxnCode = RxnCode == null
                    ? Array.Empty<RxnCode>()
                    : Array.ConvertAll(RxnCode, r => r.DeepClone() as IRxnCode),
                TextContent = TextContent == null
                    ? Array.Empty<TextContent>()
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
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

            var graphicNodes = node.SelectNodes("graphic", mgr);
            if (graphicNodes != null && graphicNodes.Count > 0)
            {
                var graphics = new List<Graphic>();
                foreach (XmlNode graphicNode in graphicNodes)
                {
                    if (graphicNode != null && graphicNode.HasChildNodes)
                    {
                        var newGraphic = new Graphic();
                        newGraphic.FromXml(graphicNode, mgr);
                        graphics.Add(newGraphic);
                    }
                }
                Graphic = graphics.ToArray();
            }

            var rxnCodeNodes = node.SelectNodes("rxnCode");
            if (rxnCodeNodes != null && rxnCodeNodes.Count > 0)
            {
                var rxnCodes = new List<RxnCode>();
                foreach (XmlNode rxnCodeNode in rxnCodeNodes)
                {
                    if (rxnCodeNode != null && rxnCodeNode.HasChildNodes)
                    {
                        var newRxnCode = new RxnCode();
                        newRxnCode.FromXml(rxnCodeNode, mgr);
                        rxnCodes.Add(newRxnCode);
                    }
                }
                RxnCode = rxnCodes.ToArray();
            }

            return this;
        }
    }
}
