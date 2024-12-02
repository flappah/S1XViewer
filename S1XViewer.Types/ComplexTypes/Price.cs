using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Price : ComplexTypeBase, IPrice
    {
        public string PriceNumber { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new Price
            {
                PriceNumber = PriceNumber,
                Currency = Currency
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
            var priceNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}priceNumber", mgr);
            if (priceNumberNode != null && priceNumberNode.HasChildNodes)
            {
                PriceNumber = priceNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            var currencyNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}currency", mgr);
            if (currencyNode != null && currencyNode.HasChildNodes)
            {
                Currency = currencyNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
