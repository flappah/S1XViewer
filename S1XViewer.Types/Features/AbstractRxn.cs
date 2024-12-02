using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public abstract class AbstractRxn : InformationFeatureBase, IAbstractRxN
    {
        public string CategoryOfAuthority { get; set; } = string.Empty;
        public IGraphic[] Graphic { get; set; } = Array.Empty<Graphic>();
        public IRxnCode[] RxnCode { get; set; } = Array.Empty<RxnCode>();
        public ITextContent[] TextContent { get; set; }  = Array.Empty<TextContent>();

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

            var categoryOfAuthorityNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfAuthority", mgr);
            if (categoryOfAuthorityNode != null && categoryOfAuthorityNode.HasChildNodes)
            {
                CategoryOfAuthority = categoryOfAuthorityNode.FirstChild?.InnerText ?? string.Empty;
            }

            var graphicNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}graphic", mgr);
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

            var rxnCodeNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}rxnCode", mgr);
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

            var textContentNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}textContent", mgr);
            if (textContentNodes != null && textContentNodes.Count > 0)
            {
                var textContentItems = new List<TextContent>();
                foreach (XmlNode textContentNode in textContentNodes)
                {
                    if (textContentNode != null && textContentNode.HasChildNodes)
                    {
                        var textContent = new ComplexTypes.TextContent();
                        textContent.FromXml(textContentNode, mgr, nameSpacePrefix);
                        textContentItems.Add(textContent);
                    }
                }
                TextContent = textContentItems.ToArray();
            }

            return this;
        }
    }
}
