using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class OffShoreMarkDescription : ComplexTypeBase, IOffShoreMarkDescription
    {
        public ITextContent[] TextContent { get; set; } = Array.Empty<ITextContent>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new OffShoreMarkDescription() { TextContent = TextContent };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var textContentNodes = node.SelectNodes("textContent", mgr);
            if (textContentNodes != null && textContentNodes.Count > 0)
            {
                var textContentItems = new List<TextContent>();
                foreach (XmlNode textContentNode in textContentNodes)
                {
                    if (textContentNode != null && textContentNode.HasChildNodes)
                    {
                        var textContent = new ComplexTypes.TextContent();
                        textContent.FromXml(textContentNode, mgr);
                        textContentItems.Add(textContent);
                    }
                }
                TextContent = textContentItems.ToArray();
            }

            return this;
        }
    }
}
