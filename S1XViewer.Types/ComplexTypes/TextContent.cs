using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class TextContent : ComplexTypeBase, ITextContent
    {
        public string CategoryOfText { get; set; } = string.Empty;
        public IInformation[] Information { get; set; } = Array.Empty<Information>();
        public IOnlineResource OnlineResource { get; set; } = new OnlineResource();
        public ISourceIndication SourceIndication { get; set; } = new SourceIndication();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new TextContent
            {
                CategoryOfText = CategoryOfText,
                Information = Information == null 
                    ? Array.Empty<Information>()
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                OnlineResource = OnlineResource == null 
                    ? new OnlineResource()
                    : OnlineResource.DeepClone() as IOnlineResource,
                SourceIndication = SourceIndication == null 
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication
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
            var categoryOfTextNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfText", mgr);
            if (categoryOfTextNode != null && categoryOfTextNode.HasChildNodes)
            {
                CategoryOfText = categoryOfTextNode.FirstChild?.InnerText ?? string.Empty;
            }

            var informationNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}information", mgr);
            if (informationNodes != null && informationNodes.Count > 0)
            {
                var informations = new List<Information>();
                foreach(XmlNode informationNode in informationNodes)
                {
                    if (informationNode != null && informationNode.HasChildNodes)
                    {
                        var newInformation = new Information();
                        newInformation.FromXml(informationNode, mgr, nameSpacePrefix);
                        informations.Add(newInformation);
                    }
                }
                Information = informations.ToArray();
            }

            var onlineResourceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}onlineResource", mgr);
            if (onlineResourceNode != null && onlineResourceNode.HasChildNodes)
            {
                OnlineResource = new OnlineResource();
                OnlineResource.FromXml(onlineResourceNode, mgr, nameSpacePrefix);
            }

            var sourceIndicationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}sourceIndication", mgr);
            if (sourceIndicationNode != null && sourceIndicationNode.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndicationNode, mgr, nameSpacePrefix);
            }

            return this;
        }
    }
}
