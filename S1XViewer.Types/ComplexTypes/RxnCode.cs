using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class RxnCode : ComplexTypeBase, IRxnCode
    {
        public string CategoryOfRxn { get; set; } = string.Empty;
        public string ActionOrActivity { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new RxnCode
            {
                CategoryOfRxn = CategoryOfRxn,
                ActionOrActivity = ActionOrActivity,
                Headline = Headline
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
            var categoryOfRxnNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfRxn", mgr);
            if (categoryOfRxnNode != null && categoryOfRxnNode.HasChildNodes)
            {
                CategoryOfRxn = categoryOfRxnNode.FirstChild?.InnerText ?? string.Empty;
            }

            var actionOrActivityNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}actionOrActivity", mgr);
            if (actionOrActivityNode != null && actionOrActivityNode.HasChildNodes)
            {
                ActionOrActivity = actionOrActivityNode.FirstChild?.InnerText ?? string.Empty;
            }

            var headlineNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}headline", mgr);
            if (headlineNode != null && headlineNode.HasChildNodes)
            {
                Headline = headlineNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
