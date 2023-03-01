using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Payment : ComplexTypeBase, IPayment
    {
        public string PriceNumber { get; set; }
        public string Currency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new Payment
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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var priceNumberNode = node.SelectSingleNode("priceNumber");
            if (priceNumberNode != null && priceNumberNode.HasChildNodes)
            {
                PriceNumber = priceNumberNode.FirstChild.InnerText;
            }

            var currencyNode = node.SelectSingleNode("currency");
            if (currencyNode != null && currencyNode.HasChildNodes)
            {
                Currency = currencyNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
