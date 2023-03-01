using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class RadioStationCommunicationDescription : ComplexTypeBase, IRadioStationCommunicationDescription
    {
        public string[] CategoryOfMaritimeBroadcast { get; set; }
        public string[] CommunicationChannel { get; set; }
        public string SignalFrequency { get; set; }
        public string TransmissionContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new RadioStationCommunicationDescription
            {
                CategoryOfMaritimeBroadcast = CategoryOfMaritimeBroadcast == null
                    ? new string[0]
                    : Array.ConvertAll(CategoryOfMaritimeBroadcast, s => s),
                CommunicationChannel = CommunicationChannel == null
                    ? new string[0]
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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var categoryOfMaritimeBroadcastNodes = node.SelectNodes("categoryOfMaritimeBroadcast", mgr);
            if (categoryOfMaritimeBroadcastNodes != null && categoryOfMaritimeBroadcastNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfMaritimeBroadcastNode in categoryOfMaritimeBroadcastNodes)
                {
                    if (categoryOfMaritimeBroadcastNode != null && categoryOfMaritimeBroadcastNode.HasChildNodes)
                    {
                        categories.Add(categoryOfMaritimeBroadcastNode.FirstChild.InnerText);
                    }
                }
                CategoryOfMaritimeBroadcast = categories.ToArray();
            }

            var communicationChannelNodes = node.SelectNodes("communicationChannel", mgr);
            if (communicationChannelNodes != null && communicationChannelNodes.Count > 0)
            {
                var channels = new List<string>();
                foreach (XmlNode communicationChannelNode in communicationChannelNodes)
                {
                    if (communicationChannelNode != null && communicationChannelNode.HasChildNodes)
                    {
                        channels.Add(communicationChannelNode.FirstChild.InnerText);
                    }
                }
                CommunicationChannel = channels.ToArray();
            }

            var signalFrequencyNode = node.SelectSingleNode("signalFrequency");
            if (signalFrequencyNode != null && signalFrequencyNode.HasChildNodes)
            {
                SignalFrequency = signalFrequencyNode.FirstChild.InnerText;
            }

            var transmissionContentNode = node.SelectSingleNode("transmissionContent");
            if (transmissionContentNode != null && transmissionContentNode.HasChildNodes)
            {
                TransmissionContent = transmissionContentNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
