using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class PeriodicDateRange : ComplexTypeBase, IPeriodicDateRange
    {
        public DateTime DateEnd { get; set; } = DateTime.MinValue;
        public DateTime DateStart { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new PeriodicDateRange()
            {
                DateEnd = DateEnd,
                DateStart = DateStart
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
            //public DateTime DateEnd { get; set; } = DateTime.MinValue;
            var dateEndNode = node.SelectSingleNode("dateEnd", mgr);
            if (dateEndNode == null)
            {
                dateEndNode = node.SelectSingleNode("S128:dateEnd", mgr);
            }
            if (dateEndNode != null && dateEndNode.HasChildNodes)
            {
                if (DateTime.TryParse(dateEndNode.FirstChild?.InnerText, out DateTime dateEndNodeValue))
                {
                    DateEnd = dateEndNodeValue;
                }
            }

            //public DateTime DateStart { get; set; } = DateTime.MinValue;
            var dateStartNode = node.SelectSingleNode("dateStart", mgr);
            if (dateStartNode == null)
            {
                dateStartNode = node.SelectSingleNode("S128:dateStart", mgr);
            }
            if (dateStartNode != null && dateStartNode.HasChildNodes)
            {
                if (DateTime.TryParse(dateStartNode.FirstChild?.InnerText, out DateTime dateStartValue))
                {
                    DateStart = dateStartValue;
                }
            }

            return this;
        }
    }
}
