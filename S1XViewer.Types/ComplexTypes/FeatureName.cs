using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class FeatureName : ComplexTypeBase, IFeatureName
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new FeatureName
            {
                DisplayName = DisplayName,
                Language = Language,
                Name = Name
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
            var displayNameNode = node.SelectSingleNode("displayName", mgr);
            if (displayNameNode != null && displayNameNode.HasChildNodes)
            {                
                DisplayName = displayNameNode.FirstChild.InnerText;
            }

            var languageNode = node.SelectSingleNode("language", mgr);
            if (languageNode != null && languageNode.HasChildNodes)
            {
                Language = languageNode.FirstChild.InnerText;
            }

            var nameNode = node.SelectSingleNode("name", mgr);
            if (nameNode != null && nameNode.HasChildNodes)
            {
                Name = nameNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
