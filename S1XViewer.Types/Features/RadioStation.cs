using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class RadioStation : GeoFeatureBase, IRadioStation, IS123Feature
    {
        public string CallSign { get; set; } = string.Empty;
        public string CategoryOfRadioStation { get; set; } = string.Empty;
        public string EstimatedRangeOffTransmission { get; set; } = string.Empty;
        public IOrientation Orientation { get; set; } = new Orientation();
        public IRadioStationCommunicationDescription[] RadioStationCommunicationDescription { get; set; } = Array.Empty<RadioStationCommunicationDescription>();
        public string Status { get; set; } = string.Empty;

        /// <summary>
        ///     Deep clones the object
        /// </summary>
        /// <returns>IComplexType</returns>
        public override IFeature DeepClone()
        {
            return new RadioStation
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
                CategoryOfRadioStation = CategoryOfRadioStation,
                EstimatedRangeOffTransmission = EstimatedRangeOffTransmission,
                Orientation = Orientation == null
                    ? new Orientation()
                    : Orientation.DeepClone() as IOrientation,
                RadioStationCommunicationDescription = RadioStationCommunicationDescription == null
                    ? Array.Empty<RadioStationCommunicationDescription>()
                    : Array.ConvertAll(RadioStationCommunicationDescription, rcd => rcd.DeepClone() as IRadioStationCommunicationDescription),
                Status = Status,
                Links = Links == null
                    ? Array.Empty<Link>()
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
            };
        }

        /// <summary>
        ///     Reads the data from an XML dom
        /// </summary>
        /// <param name="node">current node to use as a starting point for reading</param>
        /// <param name="mgr">xml namespace manager</param>
        /// <returns>IFeature</returns>
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr);

            var callSignNode = node.SelectSingleNode("callSign", mgr);
            if (callSignNode != null && callSignNode.HasChildNodes)
            {
                CallSign = callSignNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfRadioStationNode = node.SelectSingleNode("categoryOfRadioStation", mgr);
            if (categoryOfRadioStationNode != null && categoryOfRadioStationNode.HasChildNodes)
            {
                CategoryOfRadioStation = categoryOfRadioStationNode.FirstChild?.InnerText ?? string.Empty;
            }

            var estimatedRangeOffTransmissionNode = node.SelectSingleNode("estimatedRangeOffTransmission", mgr);
            if (estimatedRangeOffTransmissionNode != null && estimatedRangeOffTransmissionNode.HasChildNodes)
            {
                EstimatedRangeOffTransmission = estimatedRangeOffTransmissionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var orientationNode = node.SelectSingleNode("orientation", mgr);
            if (orientationNode != null && orientationNode.HasChildNodes)
            {
                Orientation = new Orientation();
                Orientation.FromXml(orientationNode, mgr);
            }

            var radioStationCommunicationDescriptionNodes = node.SelectNodes("radioStationCommunicationDescription", mgr);
            if (radioStationCommunicationDescriptionNodes != null && radioStationCommunicationDescriptionNodes.Count > 0)
            {
                var rdoComDescriptions = new List<RadioStationCommunicationDescription>();
                foreach (XmlNode radioStationCommunicationDescriptionNode in radioStationCommunicationDescriptionNodes)
                {
                    if (radioStationCommunicationDescriptionNode != null && radioStationCommunicationDescriptionNode.HasChildNodes)
                    {
                        var rdoComDescription = new RadioStationCommunicationDescription();
                        rdoComDescription.FromXml(radioStationCommunicationDescriptionNode, mgr);
                        rdoComDescriptions.Add(rdoComDescription);
                    }
                }
                RadioStationCommunicationDescription = rdoComDescriptions.ToArray();
            }

            var statusNode = node.SelectSingleNode("status", mgr);
            if (statusNode != null && statusNode.HasChildNodes)
            {
                Status = statusNode.FirstChild?.InnerText ?? string.Empty;  
            }

            return this;
        }
    }
}
