using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class SurveyDateRange : ComplexTypeBase, ISurveyDateRange
    {
        public string DateEnd { get; set; }
        public string DateStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new SurveyDateRange
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
            var dateEndNode = node.SelectSingleNode("dateEnd", mgr);
            if (dateEndNode != null && dateEndNode.HasChildNodes)
            {
                DateEnd = dateEndNode.FirstChild.InnerText;
            }

            var dateStartNode = node.SelectSingleNode("dateStart", mgr);
            if (dateStartNode != null && dateStartNode.HasChildNodes)
            {
                DateStart = dateStartNode.InnerText;
            }

            return this;
        }
    }
}
