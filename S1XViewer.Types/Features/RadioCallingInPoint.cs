using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class RadioCallingInPoint : GeoFeatureBase, IRadioCallingInPoint, IS127Feature
    {
        public string CallSign { get; set; } = string.Empty;
        public string[] CommunicationChannel { get; set; } = Array.Empty<string>();
        public string[] CategoryOfCargo { get; set; } = Array.Empty<string>();
        public string CategoryOfVessel { get; set; } = string.Empty;
        public IOrientation[] Orientation { get; set; } = Array.Empty<IOrientation>();
        public string[] Status { get; set; }= Array.Empty<string>();
        public string TrafficFlow { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new RadioCallingInPoint
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null
                    ? new DateRange()
                    : FixedDateRange.DeepClone() as IDateRange,
                Id = Id,
                PeriodicDateRange = PeriodicDateRange == null
                    ? Array.Empty<DateRange>()
                    : Array.ConvertAll(PeriodicDateRange, p => p.DeepClone() as IDateRange),
                SourceIndication = SourceIndication == null
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                TextContent = TextContent == null
                    ? Array.Empty<TextContent>()
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                CallSign = CallSign,
                CommunicationChannel = CommunicationChannel == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CommunicationChannel, s => s),
                CategoryOfCargo = CategoryOfCargo == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfCargo, s => s),
                CategoryOfVessel = CategoryOfVessel,
                Orientation = Orientation == null
                    ? Array.Empty<Orientation>()
                    : Array.ConvertAll(Orientation, o => o.DeepClone() as IOrientation),
                Status = Status == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(Status, s => s),
                TrafficFlow = TrafficFlow,
                Links = Links == null
                    ? Array.Empty<Link>()
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
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

            var categoryOfCargoNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfCargo", mgr);
            if (categoryOfCargoNodes != null && categoryOfCargoNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfCargoNode in categoryOfCargoNodes)
                {
                    if (categoryOfCargoNode != null && categoryOfCargoNode.HasChildNodes)
                    {
                        categories.Add(categoryOfCargoNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                CategoryOfCargo = categories.ToArray();
            }

            var categoryOfVesselNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfVessel", mgr);
            if (categoryOfVesselNode != null && categoryOfVesselNode.HasChildNodes)
            {
                CategoryOfVessel = categoryOfVesselNode.FirstChild?.InnerText ?? string.Empty;
            }

            var orientationNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}orientation", mgr);
            if (orientationNodes != null && orientationNodes.Count > 0)
            {
                var orientations = new List<Orientation>();
                foreach (XmlNode orientationNode in orientationNodes)
                {
                    if (orientationNode != null && orientationNode.HasChildNodes)
                    {
                        var newOrientation = new Orientation();
                        newOrientation.FromXml(orientationNode, mgr, nameSpacePrefix);
                        orientations.Add(newOrientation);
                    }
                }
                Orientation = orientations.ToArray();
            }

            var statusNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}status", mgr);
            if (statusNodes != null && statusNodes.Count > 0)
            {
                var statuses = new List<string>();
                foreach (XmlNode statusNode in statusNodes)
                {
                    if (statusNode != null && statusNode.HasChildNodes)
                    {
                        statuses.Add(statusNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                Status = statuses.ToArray();
            }

            var trafficFlowNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}trafficFlow", mgr);
            if (trafficFlowNode != null && trafficFlowNode.HasChildNodes)
            {
                TrafficFlow = trafficFlowNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            return "";
        }

    }
}
