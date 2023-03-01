using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class OnlineResource : ComplexTypeBase, IOnlineResource
    {
        public string ApplicationProfile { get; set; }
        public string Linkage { get; set; }
        public string NameOfResource { get; set; }
        public string OnlineDescription { get; set; }
        public string OnlineFunction { get; set; }
        public string Protocol { get; set; }
        public string ProtocolRequest { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var applicationProfileNode = node.SelectSingleNode("applicationProfile", mgr);
            if (applicationProfileNode != null && applicationProfileNode.HasChildNodes)
            {
                ApplicationProfile = applicationProfileNode.FirstChild.InnerText;
            }

            var linkageNode = node.SelectSingleNode("linkage", mgr);
            if (linkageNode != null && linkageNode.HasChildNodes)
            {
                Linkage = linkageNode.FirstChild.InnerText;
            }

            var nameOfResourceNode = node.SelectSingleNode("nameOfResource", mgr);
            if (nameOfResourceNode != null && nameOfResourceNode.HasChildNodes)
            {
                NameOfResource = nameOfResourceNode.FirstChild.InnerText;
            }

            var onlineDescriptionNode = node.SelectSingleNode("onlineDescription", mgr);
            if (onlineDescriptionNode != null && onlineDescriptionNode.HasChildNodes)
            {
                OnlineDescription = onlineDescriptionNode.FirstChild.InnerText;
            }

            var onlineFunctionNode = node.SelectSingleNode("onlineFunction", mgr);
            if (onlineFunctionNode != null && onlineFunctionNode.HasChildNodes)
            {
                OnlineFunction = onlineFunctionNode.FirstChild.InnerText;
            }

            var protocolNode = node.SelectSingleNode("protocol", mgr);
            if (protocolNode != null && protocolNode.HasChildNodes)
            {
                Protocol = protocolNode.FirstChild.InnerText;
            }

            var protocolRequestNode = node.SelectSingleNode("protocolRequest", mgr);
            if (protocolRequestNode != null && protocolRequestNode.HasChildNodes)
            {
                ProtocolRequest = protocolRequestNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
