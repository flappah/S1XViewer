using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class SurveyDateRange : ComplexTypeBase, ISurveyDateRange
    {
        public string DateEnd { get; set; } = string.Empty;
        public string DateStart { get; set; } = string.Empty;

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var dateEndNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}dateEnd", mgr);
            if (dateEndNode != null && dateEndNode.HasChildNodes)
            {
                DateEnd = dateEndNode.FirstChild?.InnerText ?? string.Empty;
            }

            var dateStartNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}dateStart", mgr);
            if (dateStartNode != null && dateStartNode.HasChildNodes)
            {
                DateStart = dateStartNode.InnerText;
            }

            return this;
        }
    }
}
