using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class FacilitiesLayoutDescription : ComplexTypeBase, IFacilitiesLayoutDescription
    {
        public ITextContent[] TextContent { get; set; } = Array.Empty<ITextContent>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new FacilitiesLayoutDescription()
            {
                TextContent = TextContent == null
                    ? new TextContent[0]
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent)
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
            if (node == null)
                return this;

            if (mgr == null)
                return this;

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
