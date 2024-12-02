using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class RadioCommunications : ComplexTypeBase, IRadioCommunications
    {
        public string CategoryOfCommPref { get; set; } = string.Empty;
        public string[] CategoryOfMaritimeBroadcast { get; set; } = Array.Empty<string>();
        public string[] CategoryOfRadioMethods { get; set; } = Array.Empty<string>();
        public string[] CommunicationChannel { get; set; } = Array.Empty<string>();
        public string ContactInstructions { get; set; } = string.Empty;
        public IFacsimileDrumSpeed FacsimileDrumSpeed { get; set; } = new FacsimileDrumSpeed();
        public IFrequencyPair[] FrequencyPair { get; set; } = Array.Empty<IFrequencyPair>();
        public ITmIntervalsByDoW[] TmIntervalsByDoW { get; set; } = Array.Empty<ITmIntervalsByDoW>();
        public string SelectiveCallNumber { get; set; } = string.Empty;
        public string SignalFrequency { get; set; } = string.Empty;
        public ITimeOfObservation TimeOfObservation { get; set; } = new TimeOfObservation();
        public ITimesOfTransmission TimesOfTransmission { get; set; } = new TimesOfTransmission();
        public string TransmissionContent { get; set; } = string.Empty;
        public string[] TransmissionRegularity { get; set; } = Array.Empty<string>();

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
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfMaritimeBroadcast, s => s),
                CategoryOfRadioMethods = CategoryOfRadioMethods == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfRadioMethods, s => s),
                CommunicationChannel = CommunicationChannel == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CommunicationChannel, s => s),
                ContactInstructions = ContactInstructions,
                FacsimileDrumSpeed = FacsimileDrumSpeed == null
                    ? new FacsimileDrumSpeed()
                    : FacsimileDrumSpeed.DeepClone() as IFacsimileDrumSpeed,
                FrequencyPair = FrequencyPair == null
                    ? Array.Empty<IFrequencyPair>()
                    : Array.ConvertAll(FrequencyPair, f => f.DeepClone() as IFrequencyPair),
                TmIntervalsByDoW = TmIntervalsByDoW == null
                    ? Array.Empty<ITmIntervalsByDoW>()
                    : Array.ConvertAll(TmIntervalsByDoW, t => t.DeepClone() as ITmIntervalsByDoW),
                SelectiveCallNumber = SelectiveCallNumber,
                SignalFrequency = SignalFrequency,
                TimeOfObservation = TimeOfObservation,
                TimesOfTransmission = TimesOfTransmission,
                TransmissionContent = TransmissionContent,
                TransmissionRegularity = TransmissionRegularity == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(TransmissionRegularity, s => s)
            };
        }

        /// <summary>
        ///     Reads the data from an XML dom
        /// </summary>
        /// <param name="node">current node to use as a starting point for reading</param>
        /// <param name="mgr">xml namespace manager</param>
        /// <returns>IFeature</returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var categoryOfCommPrefNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfCommPref", mgr);
            if (categoryOfCommPrefNode != null && categoryOfCommPrefNode.HasChildNodes)
            {
                CategoryOfCommPref = categoryOfCommPrefNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfMaritimeBroadcastNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfMaritimeBroadcast", mgr);
            if (categoryOfMaritimeBroadcastNodes != null && categoryOfMaritimeBroadcastNodes.Count > 0)
            {
                var nodes = new List<string>();
                foreach (XmlNode categoryOfMaritimeBroadcastNode in categoryOfMaritimeBroadcastNodes)
                {
                    if (categoryOfMaritimeBroadcastNode != null && categoryOfMaritimeBroadcastNode.HasChildNodes)
                    {
                        nodes.Add(categoryOfMaritimeBroadcastNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                CategoryOfMaritimeBroadcast = nodes.ToArray();
            }

            var categoryOfRadioMethodsNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfRadioMethods", mgr);
            if (categoryOfRadioMethodsNodes != null && categoryOfRadioMethodsNodes.Count > 0)
            {
                var nodes = new List<string>();
                foreach (XmlNode categoryOfRadioMethodsNode in categoryOfRadioMethodsNodes)
                {
                    if (categoryOfRadioMethodsNode != null && categoryOfRadioMethodsNode.HasChildNodes)
                    {
                        nodes.Add(categoryOfRadioMethodsNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                CategoryOfRadioMethods = nodes.ToArray();
            }

            var communicationChannelNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}communicationChannel", mgr);
            if (communicationChannelNodes != null && communicationChannelNodes.Count > 0)
            {
                var nodes = new List<string>();
                foreach(XmlNode communicationChannelNode in communicationChannelNodes)
                {
                    if (communicationChannelNode != null && communicationChannelNode.HasChildNodes)
                    {
                        nodes.Add(communicationChannelNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                CommunicationChannel = nodes.ToArray();
            }

            var contactInstructionsNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}contactInstructions", mgr);
            if (contactInstructionsNode != null && contactInstructionsNode.HasChildNodes)
            {
                ContactInstructions = contactInstructionsNode.FirstChild?.InnerText ?? string.Empty;
            }

            var facsimileDrumSpeedNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}facsimileDrumSpeed", mgr);
            if (facsimileDrumSpeedNode != null && facsimileDrumSpeedNode.HasChildNodes)
            {
                FacsimileDrumSpeed = new FacsimileDrumSpeed();
                FacsimileDrumSpeed.FromXml(facsimileDrumSpeedNode, mgr, nameSpacePrefix);
            }

            var frequencyPairNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}frequencyPair", mgr);
            if (frequencyPairNodes != null && frequencyPairNodes.Count > 0)
            {
                var frequencyPairs = new List<FrequencyPair>();
                foreach (XmlNode frequencyPairNode in frequencyPairNodes)
                {
                    if (frequencyPairNode != null && frequencyPairNode.HasChildNodes)
                    {
                        var newFrequencyPair = new FrequencyPair();
                        newFrequencyPair.FromXml(frequencyPairNode, mgr, nameSpacePrefix);
                        frequencyPairs.Add(newFrequencyPair);
                    }
                }
                FrequencyPair = frequencyPairs.ToArray();
            }

            var tmIntervalsByDoWNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}tmIntervalsByDoW", mgr);
            if (tmIntervalsByDoWNodes != null && tmIntervalsByDoWNodes.Count > 0)
            {
                var tmIntervals = new List<TmIntervalsByDoW>();
                foreach (XmlNode tmIntervalsByDoWNode in tmIntervalsByDoWNodes)
                {
                    if (tmIntervalsByDoWNode != null && tmIntervalsByDoWNode.HasChildNodes)
                    {
                        var newTmInterval = new TmIntervalsByDoW();
                        newTmInterval.FromXml(tmIntervalsByDoWNode, mgr, nameSpacePrefix);
                        tmIntervals.Add(newTmInterval);
                    }
                }
                TmIntervalsByDoW = tmIntervals.ToArray();
            }

            var selectiveCallNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}selectiveCallNumber", mgr);
            if (selectiveCallNumberNode != null && selectiveCallNumberNode.HasChildNodes)
            {
                SelectiveCallNumber = selectiveCallNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            var signalFrequencyNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}signalFrequency", mgr);
            if (signalFrequencyNode != null && signalFrequencyNode.HasChildNodes)
            {
                SignalFrequency = signalFrequencyNode.FirstChild?.InnerText ?? string.Empty;
            }

            var timeOfObservationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}timeOfObservation", mgr);
            if (timeOfObservationNode != null && timeOfObservationNode.HasChildNodes)
            {
                TimeOfObservation = new TimeOfObservation();
                TimeOfObservation.FromXml(timeOfObservationNode, mgr, nameSpacePrefix);
            }

            var timesOfTransmissionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}timesOfTransmission", mgr);
            if (timesOfTransmissionNode != null && timesOfTransmissionNode.HasChildNodes)
            {
                TimesOfTransmission = new TimesOfTransmission();
                TimesOfTransmission.FromXml(timesOfTransmissionNode, mgr, nameSpacePrefix);
            }

            var transmissionContentNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}transmissionContent", mgr);
            if (transmissionContentNode != null && transmissionContentNode.HasChildNodes)
            {
                TransmissionContent = transmissionContentNode.FirstChild?.InnerText ?? string.Empty;
            }

            var transmissionRegularityNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}transmissionRegularity", mgr);
            if (transmissionRegularityNodes != null && transmissionRegularityNodes.Count > 0)
            {
                var transmissions = new List<string>();
                foreach(XmlNode transmissionRegularityNode in transmissionRegularityNodes)
                {
                    transmissions.Add(transmissionRegularityNode.FirstChild?.InnerText ?? string.Empty);
                }
                TransmissionRegularity = transmissions.ToArray();
            }

            return this;
        }
    }
}
