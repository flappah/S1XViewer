using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class RadioCommunications : ComplexTypeBase, IRadioCommunications
    {
        public string CategoryOfCommPref { get; set; }
        public string[] CategoryOfMaritimeBroadcast { get; set; }
        public string[] CategoryOfRadioMethods { get; set; }
        public string[] CommunicationChannel { get; set; }
        public string ContactInstructions { get; set; }
        public IFacsimileDrumSpeed FacsimileDrumSpeed { get; set; }
        public IFrequencyPair[] FrequencyPair { get; set; }
        public ITmIntervalsByDoW[] TmIntervalsByDoW { get; set; }
        public string SelectiveCallNumber { get; set; }
        public string SignalFrequency { get; set; }
        public ITimeOfObservation TimeOfObservation { get; set; }
        public ITimesOfTransmission TimesOfTransmission { get; set; }
        public string TransmissionContent { get; set; }
        public string[] TransmissionRegularity { get; set; }

        /// <summary>
        ///     Deep clones the object
        /// </summary>
        /// <returns>IComplexType</returns>
        public override IComplexType DeepClone()
        {
            return new RadioCommunications
            {
                CategoryOfCommPref = CategoryOfCommPref,
                CategoryOfMaritimeBroadcast = CategoryOfMaritimeBroadcast == null
                    ? new string[0]
                    : Array.ConvertAll(CategoryOfMaritimeBroadcast, s => s),
                CategoryOfRadioMethods = CategoryOfRadioMethods == null
                    ? new string[0]
                    : Array.ConvertAll(CategoryOfRadioMethods, s => s),
                CommunicationChannel = CommunicationChannel == null
                    ? new string[0]
                    : Array.ConvertAll(CommunicationChannel, s => s),
                ContactInstructions = ContactInstructions,
                FacsimileDrumSpeed = FacsimileDrumSpeed == null
                    ? new FacsimileDrumSpeed()
                    : FacsimileDrumSpeed.DeepClone() as IFacsimileDrumSpeed,
                FrequencyPair = FrequencyPair == null
                    ? new IFrequencyPair[0]
                    : Array.ConvertAll(FrequencyPair, f => f.DeepClone() as IFrequencyPair),
                TmIntervalsByDoW = TmIntervalsByDoW == null
                    ? new ITmIntervalsByDoW[0]
                    : Array.ConvertAll(TmIntervalsByDoW, t => t.DeepClone() as ITmIntervalsByDoW),
                SelectiveCallNumber = SelectiveCallNumber,
                SignalFrequency = SignalFrequency,
                TimeOfObservation = TimeOfObservation,
                TimesOfTransmission = TimesOfTransmission,
                TransmissionContent = TransmissionContent,
                TransmissionRegularity = TransmissionRegularity == null
                    ? new string[0]
                    : Array.ConvertAll(TransmissionRegularity, s => s)
            };
        }

        /// <summary>
        ///     Reads the data from an XML dom
        /// </summary>
        /// <param name="node">current node to use as a starting point for reading</param>
        /// <param name="mgr">xml namespace manager</param>
        /// <returns>IFeature</returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var categoryOfCommPrefNode = node.SelectSingleNode("categoryOfCommPref", mgr);
            if (categoryOfCommPrefNode != null && categoryOfCommPrefNode.HasChildNodes)
            {
                CategoryOfCommPref = categoryOfCommPrefNode.FirstChild.InnerText;
            }

            var categoryOfMaritimeBroadcastNodes = node.SelectNodes("categoryOfMaritimeBroadcast", mgr);
            if (categoryOfMaritimeBroadcastNodes != null && categoryOfMaritimeBroadcastNodes.Count > 0)
            {
                var nodes = new List<string>();
                foreach (XmlNode categoryOfMaritimeBroadcastNode in categoryOfMaritimeBroadcastNodes)
                {
                    if (categoryOfMaritimeBroadcastNode != null && categoryOfMaritimeBroadcastNode.HasChildNodes)
                    {
                        nodes.Add(categoryOfMaritimeBroadcastNode.FirstChild.InnerText);
                    }
                }

                CategoryOfMaritimeBroadcast = nodes.ToArray();
            }

            var categoryOfRadioMethodsNodes = node.SelectNodes("categoryOfRadioMethods", mgr);
            if (categoryOfRadioMethodsNodes != null && categoryOfRadioMethodsNodes.Count > 0)
            {
                var nodes = new List<string>();
                foreach (XmlNode categoryOfRadioMethodsNode in categoryOfRadioMethodsNodes)
                {
                    if (categoryOfRadioMethodsNode != null && categoryOfRadioMethodsNode.HasChildNodes)
                    {
                        nodes.Add(categoryOfRadioMethodsNode.FirstChild.InnerText);
                    }
                }

                CategoryOfRadioMethods = nodes.ToArray();
            }

            var communicationChannelNodes = node.SelectNodes("communicationChannel", mgr);
            if (communicationChannelNodes != null && communicationChannelNodes.Count > 0)
            {
                var nodes = new List<string>();
                foreach(XmlNode communicationChannelNode in communicationChannelNodes)
                {
                    if (communicationChannelNode != null && communicationChannelNode.HasChildNodes)
                    {
                        nodes.Add(communicationChannelNode.FirstChild.InnerText);
                    }
                }

                CommunicationChannel = nodes.ToArray();
            }

            var contactInstructionsNode = node.SelectSingleNode("contactInstructions", mgr);
            if (contactInstructionsNode != null && contactInstructionsNode.HasChildNodes)
            {
                ContactInstructions = contactInstructionsNode.FirstChild.InnerText;
            }

            var facsimileDrumSpeedNode = node.SelectSingleNode("facsimileDrumSpeed", mgr);
            if (facsimileDrumSpeedNode != null && facsimileDrumSpeedNode.HasChildNodes)
            {
                FacsimileDrumSpeed = new FacsimileDrumSpeed();
                FacsimileDrumSpeed.FromXml(facsimileDrumSpeedNode, mgr);
            }

            var frequencyPairNodes = node.SelectNodes("frequencyPair", mgr);
            if (frequencyPairNodes != null && frequencyPairNodes.Count > 0)
            {
                var frequencyPairs = new List<FrequencyPair>();
                foreach (XmlNode frequencyPairNode in frequencyPairNodes)
                {
                    if (frequencyPairNode != null && frequencyPairNode.HasChildNodes)
                    {
                        var newFrequencyPair = new FrequencyPair();
                        newFrequencyPair.FromXml(frequencyPairNode, mgr);
                        frequencyPairs.Add(newFrequencyPair);
                    }
                }
                FrequencyPair = frequencyPairs.ToArray();
            }

            var tmIntervalsByDoWNodes = node.SelectNodes("tmIntervalsByDoW", mgr);
            if (tmIntervalsByDoWNodes != null && tmIntervalsByDoWNodes.Count > 0)
            {
                var tmIntervals = new List<TmIntervalsByDoW>();
                foreach (XmlNode tmIntervalsByDoWNode in tmIntervalsByDoWNodes)
                {
                    if (tmIntervalsByDoWNode != null && tmIntervalsByDoWNode.HasChildNodes)
                    {
                        var newTmInterval = new TmIntervalsByDoW();
                        newTmInterval.FromXml(tmIntervalsByDoWNode, mgr);
                        tmIntervals.Add(newTmInterval);
                    }
                }
                TmIntervalsByDoW = tmIntervals.ToArray();
            }

            var selectiveCallNumberNode = node.SelectSingleNode("selectiveCallNumber");
            if (selectiveCallNumberNode != null && selectiveCallNumberNode.HasChildNodes)
            {
                SelectiveCallNumber = selectiveCallNumberNode.FirstChild.InnerText;
            }

            var signalFrequencyNode = node.SelectSingleNode("signalFrequency");
            if (signalFrequencyNode != null && signalFrequencyNode.HasChildNodes)
            {
                SignalFrequency = signalFrequencyNode.FirstChild.InnerText;
            }

            var timeOfObservationNode = node.SelectSingleNode("timeOfObservation", mgr);
            if (timeOfObservationNode != null && timeOfObservationNode.HasChildNodes)
            {
                TimeOfObservation = new TimeOfObservation();
                TimeOfObservation.FromXml(timeOfObservationNode, mgr);
            }

            var timesOfTransmissionNode = node.SelectSingleNode("timesOfTransmission", mgr);
            if (timesOfTransmissionNode != null && timesOfTransmissionNode.HasChildNodes)
            {
                TimesOfTransmission = new TimesOfTransmission();
                TimesOfTransmission.FromXml(timesOfTransmissionNode, mgr);
            }

            var transmissionContentNode = node.SelectSingleNode("transmissionContent", mgr);
            if (transmissionContentNode != null && transmissionContentNode.HasChildNodes)
            {
                TransmissionContent = transmissionContentNode.FirstChild.InnerText;
            }

            var transmissionRegularityNodes = node.SelectNodes("transmissionRegularity", mgr);
            if (transmissionRegularityNodes != null && transmissionRegularityNodes.Count > 0)
            {
                var transmissions = new List<string>();
                foreach(XmlNode transmissionRegularityNode in transmissionRegularityNodes)
                {
                    transmissions.Add(transmissionRegularityNode.FirstChild.InnerText);
                }
                TransmissionRegularity = transmissions.ToArray();
            }

            return this;
        }
    }
}
