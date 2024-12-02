using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class ServiceSpecification : ComplexTypeBase, IServiceSpecification
    {
        public string Date { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new ServiceSpecification
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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var dateNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}date", mgr);
            if (dateNode != null && dateNode.HasChildNodes)
            {
                Date = dateNode.FirstChild?.InnerText ?? string.Empty;
            }

            var nameNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}name", mgr);
            if (nameNode != null && nameNode.HasChildNodes)
            {
                Name = node.InnerText;
            }

            var versionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}version", mgr);
            if (versionNode != null && versionNode.HasChildNodes)
            {
                Version = node.InnerText;
            }

            return this;
        }

    }
}
