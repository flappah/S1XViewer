using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Information : ComplexTypeBase, IInformation
    {
        public string FileLocator { get; set; } = string.Empty;
        public string FileReference { get; set; } = string.Empty;   
        public string Headline { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new Information
            {
                FileLocator = FileLocator,
                FileReference = FileReference,
                Headline = Headline,
                Language = Language,
                Text = Text
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
            var fileLocatorNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}fileLocator", mgr);
            if (fileLocatorNode != null && fileLocatorNode.HasChildNodes)
            {
                FileLocator = fileLocatorNode.FirstChild?.InnerText ?? string.Empty;
            }

            var fileReferenceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}fileReference", mgr);
            if (fileReferenceNode != null && fileReferenceNode.HasChildNodes)
            {
                FileReference = fileReferenceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var headlineNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}headline", mgr);
            if (headlineNode != null && headlineNode.HasChildNodes)
            {
                Headline = headlineNode.FirstChild?.InnerText ?? string.Empty;
            }

            var languageNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}language", mgr);
            if (languageNode != null && languageNode.HasChildNodes)
            {
                Language = languageNode.FirstChild?.InnerText ?? string.Empty;
            }
            var textNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}text", mgr);
            if (textNode != null && textNode.HasChildNodes)
            {   
                Text = textNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
