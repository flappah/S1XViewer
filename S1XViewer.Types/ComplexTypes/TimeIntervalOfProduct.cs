using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class TimeIntervalOfProduct : ComplexTypeBase, ITimeIntervalOfProduct
    {
        public DateTime ExpirationDate { get; set; }
        public DateTime IssueDate { get; set; }
        public IIssuanceCycle IssuanceCycle { get; set; } = new IssuanceCycle();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new TimeIntervalOfProduct()
            {
                ExpirationDate = ExpirationDate,
                IssueDate = IssueDate,
                IssuanceCycle = IssuanceCycle == null
                    ? new IssuanceCycle()
                    : IssuanceCycle.DeepClone() as IIssuanceCycle
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
            //public DateTime ExpirationDate { get; set; }
            var expirationDateNode = node.SelectSingleNode("expirationDate", mgr);
            if (expirationDateNode == null)
            {
                expirationDateNode = node.SelectSingleNode("S128:expirationDate", mgr);
            }
            if (expirationDateNode != null && expirationDateNode.HasChildNodes)
            {
                if (DateTime.TryParse(expirationDateNode.FirstChild?.InnerText, out DateTime expirationDateValue))
                {
                    ExpirationDate = expirationDateValue;
                }
            }

            //public DateTime IssueDate { get; set; }
            var issueDateNode = node.SelectSingleNode("issueDate", mgr);
            if (issueDateNode == null)
            {
                issueDateNode = node.SelectSingleNode("S128:issueDate", mgr);
            }
            if (issueDateNode != null && issueDateNode.HasChildNodes)
            {
                if (DateTime.TryParse(issueDateNode.FirstChild?.InnerText, out DateTime issueDateValue))
                {
                    IssueDate = issueDateValue;
                }
            }

            //public IIssuanceCycle IssuanceCycle { get; set; }
            var issuanceCycleNode = node.SelectSingleNode("issuanceCycle", mgr);
            if (issuanceCycleNode == null)
            {
                issuanceCycleNode = node.SelectSingleNode("S128:issuanceCycle", mgr);
            }
            if (issuanceCycleNode != null && issuanceCycleNode.HasChildNodes)
            {
                IssuanceCycle = new IssuanceCycle();
                IssuanceCycle.FromXml(issuanceCycleNode, mgr);
            }

            return this;
        }
    }
}
