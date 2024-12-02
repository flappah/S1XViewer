using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class ConstructionInformation : ComplexTypeBase, IConstructionInformation
    {
        public IDateRange FixedDateRange { get; set; } = new DateRange();
        public string Condition { get; set; } = string.Empty;
        public string Development { get; set; } = string.Empty;
        public string LocationByText { get; set; } = string.Empty;
        public ITextContent[] TextContent { get; set; } = Array.Empty<ITextContent>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new ConstructionInformation()
            {
                FixedDateRange = FixedDateRange,
                Condition = Condition,
                Development = Development,
                LocationByText = LocationByText,
                TextContent = TextContent == null
                    ? Array.Empty<TextContent>()
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            var fixedDateRangeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}fixedDateRange", mgr);
            if (fixedDateRangeNode != null && fixedDateRangeNode.HasChildNodes)
            {
                FixedDateRange = new DateRange();
                FixedDateRange.FromXml(fixedDateRangeNode, mgr);
            }

            var conditionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}condition", mgr);
            if (conditionNode != null && conditionNode.HasChildNodes)
            {
                Condition = conditionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var developmentNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}development", mgr);
            if (developmentNode != null && developmentNode.HasChildNodes)
            {
                Development = developmentNode.FirstChild?.InnerText ?? string.Empty;
            }

            var locationByTextNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}locationByText", mgr);
            if (locationByTextNode != null && locationByTextNode.HasChildNodes)
            {
                LocationByText = locationByTextNode.FirstChild?.InnerText ?? string.Empty;
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
