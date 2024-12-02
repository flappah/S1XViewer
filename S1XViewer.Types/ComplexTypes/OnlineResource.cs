using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class OnlineResource : ComplexTypeBase, IOnlineResource
    {
        public string ApplicationProfile { get; set; } = string.Empty;
        public string Linkage { get; set; } = string.Empty;
        public string NameOfResource { get; set; } = string.Empty;
        public string OnlineDescription { get; set; } = string.Empty;
        public string OnlineFunction { get; set; } = string.Empty;
        public string Protocol { get; set; } = string.Empty;
        public string ProtocolRequest { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new OnlineResource
            {
                ApplicationProfile = ApplicationProfile,
                Linkage = Linkage,
                NameOfResource = NameOfResource,
                OnlineDescription = OnlineDescription,
                OnlineFunction = OnlineFunction,
                Protocol = Protocol,
                ProtocolRequest  = ProtocolRequest
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
            var applicationProfileNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}applicationProfile", mgr);
            if (applicationProfileNode != null && applicationProfileNode.HasChildNodes)
            {
                ApplicationProfile = applicationProfileNode.FirstChild?.InnerText ?? string.Empty;
            }

            var linkageNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}linkage", mgr);
            if (linkageNode != null && linkageNode.HasChildNodes)
            {
                Linkage = linkageNode.FirstChild?.InnerText ?? string.Empty;
            }

            var nameOfResourceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}nameOfResource", mgr);
            if (nameOfResourceNode != null && nameOfResourceNode.HasChildNodes)
            {
                NameOfResource = nameOfResourceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var onlineDescriptionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}onlineDescription", mgr);
            if (onlineDescriptionNode != null && onlineDescriptionNode.HasChildNodes)
            {
                OnlineDescription = onlineDescriptionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var onlineFunctionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}onlineFunction", mgr);
            if (onlineFunctionNode != null && onlineFunctionNode.HasChildNodes)
            {
                OnlineFunction = onlineFunctionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var protocolNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}protocol", mgr);
            if (protocolNode != null && protocolNode.HasChildNodes)
            {
                Protocol = protocolNode.FirstChild?.InnerText ?? string.Empty;
            }

            var protocolRequestNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}protocolRequest", mgr);
            if (protocolRequestNode != null && protocolRequestNode.HasChildNodes)
            {
                ProtocolRequest = protocolRequestNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
