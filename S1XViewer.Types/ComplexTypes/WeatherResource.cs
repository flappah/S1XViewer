using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class WeatherResource : ComplexTypeBase, IWeatherResource
    {
        public IOnlineResource OnlineResource { get; set; } = new OnlineResource();
        public string DynamicResource { get; set; } = string.Empty;
        public ITextContent TextContent { get; set; } = new TextContent();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new WeatherResource()
            {
                OnlineResource = OnlineResource.DeepClone() as IOnlineResource,
                DynamicResource = DynamicResource,
                TextContent = TextContent.DeepClone() as ITextContent
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

            var onlineResourceNode = node.SelectSingleNode("onlineResource", mgr);
            if (onlineResourceNode != null && onlineResourceNode.HasChildNodes)
            {
                OnlineResource = new OnlineResource();
                OnlineResource.FromXml(onlineResourceNode, mgr);
            }

            var dynamicResourceNode = node.SelectSingleNode("dynamicResource", mgr);
            if (dynamicResourceNode != null && dynamicResourceNode.HasChildNodes)
            {
                DynamicResource = dynamicResourceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var textContentNode = node.SelectSingleNode("textContent", mgr);
            if (textContentNode != null && textContentNode.HasChildNodes)
            {
                TextContent = new TextContent();
                TextContent.FromXml(textContentNode, mgr);
            }

            return this;
        }
    }
}
