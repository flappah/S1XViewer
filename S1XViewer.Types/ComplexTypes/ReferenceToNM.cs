using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class ReferenceToNM : ComplexTypeBase, IReferenceToNM
    {
        public string Week { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;

        public override IComplexType DeepClone()
        {
            return new ReferenceToNM
            {
                Week = Week,
                Year = Year
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
            var weekNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}week", mgr);
            if (weekNode != null && weekNode.HasChildNodes)
            {
                Week = weekNode.FirstChild?.InnerText ?? string.Empty;
            }

            var yearNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}year", mgr);
            if (yearNode != null && yearNode.HasChildNodes)
            {
                Year = yearNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
