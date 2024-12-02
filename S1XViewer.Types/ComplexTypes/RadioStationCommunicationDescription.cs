using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class RadioStationCommunicationDescription : ComplexTypeBase, IRadioStationCommunicationDescription
    {
        public string[] CategoryOfMaritimeBroadcast { get; set; } = Array.Empty<string>();
        public string[] CommunicationChannel { get; set; } = Array.Empty<string>();
        public string SignalFrequency { get; set; } = string.Empty;
        public string TransmissionContent { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new RadioStationCommunicationDescription
            {
                CategoryOfMaritimeBroadcast = CategoryOfMaritimeBroadcast == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfMaritimeBroadcast, s => s),
                CommunicationChannel = CommunicationChannel == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CommunicationChannel, s => s),
                SignalFrequency = SignalFrequency,
                TransmissionContent = TransmissionContent
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
            var categoryOfMaritimeBroadcastNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfMaritimeBroadcast", mgr);
            if (categoryOfMaritimeBroadcastNodes != null && categoryOfMaritimeBroadcastNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfMaritimeBroadcastNode in categoryOfMaritimeBroadcastNodes)
                {
                    if (categoryOfMaritimeBroadcastNode != null && categoryOfMaritimeBroadcastNode.HasChildNodes)
                    {
                        categories.Add(categoryOfMaritimeBroadcastNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                CategoryOfMaritimeBroadcast = categories.ToArray();
            }

            var communicationChannelNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}communicationChannel", mgr);
            if (communicationChannelNodes != null && communicationChannelNodes.Count > 0)
            {
                var channels = new List<string>();
                foreach (XmlNode communicationChannelNode in communicationChannelNodes)
                {
                    if (communicationChannelNode != null && communicationChannelNode.HasChildNodes)
                    {
                        channels.Add(communicationChannelNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                CommunicationChannel = channels.ToArray();
            }

            var signalFrequencyNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}signalFrequency", mgr);
            if (signalFrequencyNode != null && signalFrequencyNode.HasChildNodes)
            {
                SignalFrequency = signalFrequencyNode.FirstChild?.InnerText ?? string.Empty;
            }

            var transmissionContentNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}transmissionContent", mgr);
            if (transmissionContentNode != null && transmissionContentNode.HasChildNodes)
            {
                TransmissionContent = transmissionContentNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
