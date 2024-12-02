using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class PilotBoardingPlace : Layout, IPilotBoardingPlace, IS127Feature, IS131Feature
    {
        public string CallSign { get; set; } = string.Empty;
        public string CategoryOfPilotBoardingPlace { get; set; } = string.Empty;
        public string CategoryOfPreference { get; set; } = string.Empty;
        public string CategoryOfVessel { get; set; } = string.Empty;
        public string[] CommunicationChannel { get; set; } = new string[0];
        public string Destination { get; set; } = string.Empty;
        public string PilotMovement { get; set; } = string.Empty;
        public string PilotVessel { get; set; } = string.Empty;
        public string[] Status { get; set; } = new string[0];

        //S131
        public IDepthsDescription DepthsDescription { get; set; } = new DepthsDescription();
        public string LocationByText {  get; set; } = string.Empty;
        public IMarkedBy MarkedBy { get; set; } = new MarkedBy();
        public string ISPSLevel { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new PilotBoardingPlace
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
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                TextContent = TextContent == null
                    ? new TextContent[0]
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                Links = Links == null
                    ? new Link[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink),
                CallSign = CallSign,
                CategoryOfPilotBoardingPlace = CategoryOfPilotBoardingPlace,
                CategoryOfPreference = CategoryOfPreference,
                CategoryOfVessel = CategoryOfVessel,
                CommunicationChannel = CommunicationChannel == null
                    ? new string[0]
                    : Array.ConvertAll(CommunicationChannel, s => s),
                Destination = Destination,
                PilotMovement = PilotMovement,
                PilotVessel = PilotVessel,
                Status = Status == null
                    ? new string[0]
                    : Array.ConvertAll(Status, s => s),
                DepthsDescription = DepthsDescription == null ? new DepthsDescription() : DepthsDescription.DeepClone() as IDepthsDescription,
                LocationByText = LocationByText,
                MarkedBy = MarkedBy == null ? new MarkedBy() : MarkedBy.DeepClone() as IMarkedBy,
                ISPSLevel = ISPSLevel
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr, nameSpacePrefix);

            var callSignNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}callSign", mgr);
            if (callSignNode != null && callSignNode.HasChildNodes)
            {
                CallSign = callSignNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfPilotBoardingPlaceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfPilotBoardingPlace", mgr);
            if (categoryOfPilotBoardingPlaceNode != null && categoryOfPilotBoardingPlaceNode.HasChildNodes)
            {
                CategoryOfPilotBoardingPlace = categoryOfPilotBoardingPlaceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfPreferenceNode= node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfPreference", mgr);
            if (categoryOfPreferenceNode != null && categoryOfPreferenceNode.HasChildNodes)
            {
                CategoryOfPilotBoardingPlace = categoryOfPreferenceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfVesselNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfVessel", mgr);
            if (categoryOfVesselNode != null && categoryOfVesselNode.HasChildNodes)
            {
                CategoryOfVessel = categoryOfVesselNode.FirstChild?.InnerText ?? string.Empty;
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

            var destinationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}destination", mgr);
            if (destinationNode != null && destinationNode.HasChildNodes)
            {
                Destination = destinationNode.FirstChild?.InnerText ?? string.Empty;
            }

            var pilotMovementNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}pilotMovement", mgr);
            if (pilotMovementNode != null && pilotMovementNode.HasChildNodes)
            {
                PilotMovement = pilotMovementNode.FirstChild?.InnerText ?? string.Empty;
            }

            var pilotVesselNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}pilotVessel", mgr);
            if (pilotVesselNode != null && pilotVesselNode.HasChildNodes)
            {
                PilotVessel = pilotVesselNode.FirstChild?.InnerText ?? string.Empty;
            }

            var statusNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}status", mgr);
            if (statusNodes != null && statusNodes.Count > 0)
            {
                var statuses = new List<string>();
                foreach (XmlNode statusNode in statusNodes)
                {
                    if (statusNode != null && statusNode.HasChildNodes && String.IsNullOrEmpty(statusNode.FirstChild?.InnerText) == false)
                    {
                        statuses.Add(statusNode.FirstChild?.InnerText);
                    }
                }

                statuses.Sort();
                Status = statuses.ToArray();
            }

            var depthsDescriptionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}depthsDescription", mgr);
            if (depthsDescriptionNode != null && depthsDescriptionNode.HasChildNodes)
            {
                DepthsDescription = new DepthsDescription();
                DepthsDescription.FromXml(depthsDescriptionNode, mgr, nameSpacePrefix);
            }

            var locationByTextNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}locationByText", mgr);
            if (locationByTextNode != null && locationByTextNode.HasChildNodes)
            {
                LocationByText = locationByTextNode.FirstChild?.InnerText ?? string.Empty;
            }

            var markedByNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}markedBy", mgr);
            if (markedByNode != null && markedByNode.HasChildNodes)
            {
                MarkedBy = new MarkedBy();
                MarkedBy.FromXml(markedByNode, mgr, nameSpacePrefix);
            }

            var iSPSLevelNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}iSPSLevel", mgr);
            if (iSPSLevelNode != null && iSPSLevelNode.HasChildNodes)
            {
                ISPSLevel = iSPSLevelNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            return "PILBOP02";
        }
    }
}
