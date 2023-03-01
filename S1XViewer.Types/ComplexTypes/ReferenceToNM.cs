using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class ReferenceToNM : ComplexTypeBase, IReferenceToNM
    {
        public string Week { get; set; }
        public string Year { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var weekNode = node.SelectSingleNode("week", mgr);
            if (weekNode != null && weekNode.HasChildNodes)
            {
                Week = weekNode.FirstChild.InnerText;
            }

            var yearNode = node.SelectSingleNode("year", mgr);
            if (yearNode != null && yearNode.HasChildNodes)
            {
                Year = yearNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
