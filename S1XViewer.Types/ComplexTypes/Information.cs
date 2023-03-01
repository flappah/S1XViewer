using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Information : ComplexTypeBase, IInformation
    {
        public string FileLocator { get; set; }
        public string FileReference { get; set; }
        public string Headline { get; set; }
        public string Language { get; set; }
        public string Text { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var fileLocatorNode = node.SelectSingleNode("fileLocator", mgr);
            if (fileLocatorNode != null && fileLocatorNode.HasChildNodes)
            {
                FileLocator = fileLocatorNode.FirstChild.InnerText;
            }

            var fileReferenceNode = node.SelectSingleNode("fileReference", mgr);
            if (fileReferenceNode != null && fileReferenceNode.HasChildNodes)
            {
                FileReference = fileReferenceNode.FirstChild.InnerText;
            }

            var headlineNode = node.SelectSingleNode("headline", mgr);
            if (headlineNode != null && headlineNode.HasChildNodes)
            {
                Headline = headlineNode.FirstChild.InnerText;
            }

            var languageNode = node.SelectSingleNode("language", mgr);
            if (languageNode != null && languageNode.HasChildNodes)
            {
                Language = languageNode.FirstChild.InnerText;
            }
            var textNode = node.SelectSingleNode("text", mgr);
            if (textNode != null && textNode.HasChildNodes)
            {
                Text = textNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
