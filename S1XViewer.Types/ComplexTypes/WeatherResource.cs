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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            var onlineResourceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}onlineResource", mgr);
            if (onlineResourceNode != null && onlineResourceNode.HasChildNodes)
            {
                OnlineResource = new OnlineResource();
                OnlineResource.FromXml(onlineResourceNode, mgr, nameSpacePrefix);
            }

            var dynamicResourceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}dynamicResource", mgr);
            if (dynamicResourceNode != null && dynamicResourceNode.HasChildNodes)
            {
                DynamicResource = dynamicResourceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var textContentNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}textContent", mgr);
            if (textContentNode != null && textContentNode.HasChildNodes)
            {
                TextContent = new TextContent();
                TextContent.FromXml(textContentNode, mgr, nameSpacePrefix);
            }

            return this;
        }
    }
}
