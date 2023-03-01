using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class RxnCode : ComplexTypeBase, IRxnCode
    {
        public string CategoryOfRxn { get; set; }
        public string ActionOrActivity { get; set; }
        public string Headline { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var categoryOfRxnNode = node.SelectSingleNode("categoryOfRxn", mgr);
            if (categoryOfRxnNode != null && categoryOfRxnNode.HasChildNodes)
            {
                CategoryOfRxn = categoryOfRxnNode.FirstChild.InnerText;
            }

            var actionOrActivityNode = node.SelectSingleNode("actionOrActivity");
            if (actionOrActivityNode != null && actionOrActivityNode.HasChildNodes)
            {
                ActionOrActivity = actionOrActivityNode.FirstChild.InnerText;
            }

            var headlineNode = node.SelectSingleNode("headline");
            if (headlineNode != null && headlineNode.HasChildNodes)
            {
                Headline = headlineNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
