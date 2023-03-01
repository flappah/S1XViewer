using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class SupportFileSpecification : ComplexTypeBase, ISupportFileSpecification
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new SupportFileSpecification
            {
                Date = Date,
                Name = Name,
                Version = Version
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
            var dateNode = node.SelectSingleNode("date");
            if (dateNode != null && dateNode.HasChildNodes)
            {
                Date = dateNode.FirstChild.InnerText;
            }

            var nameNode = node.SelectSingleNode("name");
            if (nameNode != null && nameNode.HasChildNodes)
            {
                Name = nameNode.FirstChild.InnerText;
            }

            var versionNode = node.SelectSingleNode("version");
            if (versionNode != null && versionNode.HasChildNodes)
            {
                Version = versionNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
