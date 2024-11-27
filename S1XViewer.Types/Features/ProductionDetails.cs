using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class ProductionDetails : InformationFeatureBase, IProductionDetails, IS128Feature
    {
        public string CallName { get; set; } = string.Empty;
        public string CallSign { get; set; } = string.Empty;
        public string CategoryOfCommPref { get; set; } = string.Empty;
        public string[] CommunicationChannel { get; set; } = Array.Empty<string>();
        public IContactAddress[] ContactAddress { get; set; } = Array.Empty<IContactAddress>();
        public string ContactInstructions { get; set; } = string.Empty;
        public IFrequencyPair[] FrequencyPair { get; set; } = Array.Empty<FrequencyPair>();
        public IInformation[] Information { get; set; } = Array.Empty<Information>();
        public string MMsiCode { get; set; } = string.Empty;
        public IOnlineResource[] OnlineResource { get; set; } = Array.Empty<OnlineResource>();
        public IRadioCommunications[] RadioCommunications { get; set; } = Array.Empty<RadioCommunications>();
        public ITelecommunications[] Telecommunications { get; set; } = Array.Empty<Telecommunications>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new ProductionDetails
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null
                    ? new DateRange()
                    : FixedDateRange.DeepClone() as IDateRange,
                Id = Id,
                PeriodicDateRange = PeriodicDateRange == null
                    ? new DateRange[0]
                    : Array.ConvertAll(PeriodicDateRange, p => p.DeepClone() as IDateRange),
                SourceIndication = SourceIndication == null
                    ? new SourceIndication[0]
                    : Array.ConvertAll(SourceIndication, s => s.DeepClone() as ISourceIndication),
                CallName = CallName,
                CallSign = CallSign,
                CategoryOfCommPref = CategoryOfCommPref,
                CommunicationChannel = CommunicationChannel == null
                    ? new string[0]
                    : Array.ConvertAll(CommunicationChannel, s => s),
                ContactInstructions = ContactInstructions,
                MMsiCode = MMsiCode,
                ContactAddress = ContactAddress == null
                    ? new ContactAddress[0]
                    : Array.ConvertAll(ContactAddress, ca => ca.DeepClone() as IContactAddress),
                Information = Information == null
                    ? new Information[0]
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                FrequencyPair = FrequencyPair == null
                    ? new FrequencyPair[0]
                    : Array.ConvertAll(FrequencyPair, fp => fp.DeepClone() as IFrequencyPair),
                OnlineResource = OnlineResource == null
                    ? new OnlineResource[0]
                    : Array.ConvertAll(OnlineResource, or => or.DeepClone() as IOnlineResource),
                RadioCommunications = RadioCommunications == null
                    ? new RadioCommunications[0]
                    : Array.ConvertAll(RadioCommunications, r => r.DeepClone() as IRadioCommunications),
                Telecommunications = Telecommunications == null
                    ? new Telecommunications[0]
                    : Array.ConvertAll(Telecommunications, t => t.DeepClone() as ITelecommunications),
                Links = Links == null
                    ? new Link[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            if (node.HasChildNodes)
            {
                if (node.Attributes?.Count > 0 &&
                    node.Attributes.Contains("gml:id") == true)
                {
                    Id = node.Attributes["gml:id"].InnerText;
                }
            }

            var fixedDateRangeNode = node.SelectSingleNode("fixedDateRange", mgr);
            if (fixedDateRangeNode != null && fixedDateRangeNode.HasChildNodes)
            {
                FixedDateRange = new DateRange();
                FixedDateRange.FromXml(fixedDateRangeNode, mgr);
            }

            var periodicDateRangeNodes = node.SelectNodes("periodicDateRange", mgr);
            if (periodicDateRangeNodes != null && periodicDateRangeNodes.Count > 0)
            {
                var dateRanges = new List<DateRange>();
                foreach (XmlNode periodicDateRangeNode in periodicDateRangeNodes)
                {
                    var newDateRange = new DateRange();
                    newDateRange.FromXml(periodicDateRangeNode, mgr);
                    dateRanges.Add(newDateRange);
                }
                PeriodicDateRange = dateRanges.ToArray();
            }

            var featureNameNodes = node.SelectNodes("featureName", mgr);
            if (featureNameNodes != null && featureNameNodes.Count > 0)
            {
                var featureNames = new List<FeatureName>();
                foreach (XmlNode featureNameNode in featureNameNodes)
                {
                    var newFeatureName = new FeatureName();
                    newFeatureName.FromXml(featureNameNode, mgr);
                    featureNames.Add(newFeatureName);
                }
                FeatureName = featureNames.ToArray();
            }

            var sourceIndicationNodes = node.SelectNodes("sourceIndication", mgr);
            if (sourceIndicationNodes != null && sourceIndicationNodes.Count > 0)
            {
                var sourceIndications = new List<SourceIndication>();
                foreach (XmlNode sourceIndicationNode in sourceIndicationNodes)
                {
                    if (sourceIndicationNode != null && sourceIndicationNode.HasChildNodes)
                    {
                        var sourceIndication = new SourceIndication();
                        sourceIndication.FromXml(sourceIndicationNode, mgr);
                        sourceIndications.Add(sourceIndication);
                    }
                }
                SourceIndication = sourceIndications.ToArray();
            }

            var callNameNode = node.SelectSingleNode("callName", mgr);
            if (callNameNode != null && callNameNode.HasChildNodes)
            {
                CallName = callNameNode.FirstChild.InnerText;
            }

            var callSignNode = node.SelectSingleNode("callSign", mgr);
            if (callSignNode != null && callSignNode.HasChildNodes)
            {
                CallSign = callSignNode.FirstChild.InnerText;
            }

            var categoryOfCommPrefNode = node.SelectSingleNode("categoryOfCommPref", mgr);
            if (categoryOfCommPrefNode != null && categoryOfCommPrefNode.HasChildNodes)
            {
                CategoryOfCommPref = categoryOfCommPrefNode.FirstChild.InnerText;
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

                channels.Sort();
                CommunicationChannel = channels.ToArray();
            }

            var contactInstructionsNode = node.SelectSingleNode("contactInstructions", mgr);
            if (contactInstructionsNode != null && contactInstructionsNode.HasChildNodes)
            {
                ContactInstructions = contactInstructionsNode.FirstChild.InnerText;
            }

            var mmsiCodeNode = node.SelectSingleNode("mMSICode", mgr);
            if (mmsiCodeNode != null && mmsiCodeNode.HasChildNodes)
            {
                MMsiCode = mmsiCodeNode.FirstChild.InnerText;
            }

            var contactAddressNodes = node.SelectNodes("contactAddress", mgr);
            if (contactAddressNodes != null && contactAddressNodes.Count > 0)
            {
                var contactAddresses = new List<ContactAddress>();
                foreach (XmlNode contactAddressNode in contactAddressNodes)
                {
                    if (contactAddressNode != null && contactAddressNode.HasChildNodes)
                    {
                        var newContactAddress = new ContactAddress();
                        newContactAddress.FromXml(contactAddressNode, mgr);
                        contactAddresses.Add(newContactAddress);
                    }
                }
                ContactAddress = contactAddresses.ToArray();
            }

            var informationNodes = node.SelectNodes("information", mgr);
            if (informationNodes != null && informationNodes.Count > 0)
            {
                var informations = new List<Information>();
                foreach (XmlNode informationNode in informationNodes)
                {
                    if (informationNode != null && informationNode.HasChildNodes)
                    {
                        var newInformation = new Information();
                        newInformation.FromXml(informationNode, mgr);
                        informations.Add(newInformation);
                    }
                }
                Information = informations.ToArray();
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

            var onlineResourceNodes = node.SelectNodes("onlineResource", mgr);
            if (onlineResourceNodes != null && onlineResourceNodes.Count > 0)
            {
                var onlineResources = new List<OnlineResource>();
                foreach (XmlNode onlineResourceNode in onlineResourceNodes)
                {
                    if (onlineResourceNode != null && onlineResourceNode.HasChildNodes)
                    {
                        var newOnlineResource = new OnlineResource();
                        newOnlineResource.FromXml(onlineResourceNode, mgr);
                        onlineResources.Add(newOnlineResource);
                    }
                }
                OnlineResource = onlineResources.ToArray();
            }

            var radioCommunicationNodes = node.SelectNodes("radiocommunications", mgr);
            if (radioCommunicationNodes != null && radioCommunicationNodes.Count > 0)
            {
                var radioCommunications = new List<RadioCommunications>();
                foreach (XmlNode radioCommunicationNode in radioCommunicationNodes)
                {
                    if (radioCommunicationNode != null && radioCommunicationNode.HasChildNodes)
                    {
                        var newRadioCommunications = new RadioCommunications();
                        newRadioCommunications.FromXml(radioCommunicationNode, mgr);
                        radioCommunications.Add(newRadioCommunications);
                    }
                }
                RadioCommunications = radioCommunications.ToArray();
            }

            var teleCommunicationNodes = node.SelectNodes("telecommunications", mgr);
            if (teleCommunicationNodes != null && teleCommunicationNodes.Count > 0)
            {
                var teleCommunications = new List<Telecommunications>();
                foreach (XmlNode teleCommunicationNode in teleCommunicationNodes)
                {
                    if (teleCommunicationNode != null && teleCommunicationNode.HasChildNodes)
                    {
                        var newTelecommunications = new Telecommunications();
                        newTelecommunications.FromXml(teleCommunicationNode, mgr);
                        teleCommunications.Add(newTelecommunications);
                    }
                }
                Telecommunications = teleCommunications.ToArray();
            }

            var linkNodes = node.SelectNodes("*[boolean(@xlink:href)]", mgr);
            if (linkNodes != null && linkNodes.Count > 0)
            {
                var links = new List<Link>();
                foreach (XmlNode linkNode in linkNodes)
                {
                    var newLink = new Link();
                    newLink.FromXml(linkNode, mgr);
                    links.Add(newLink);
                }
                Links = links.ToArray();
            }

            return this;
        }
    }
}
