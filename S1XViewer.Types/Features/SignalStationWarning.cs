using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class SignalStationWarning : GeoFeatureBase, ISignalStationWarning, IS127Feature
    {
        public string[] CategoryOfSignalStationWarning { get; set; } = Array.Empty<string>();
        public string[] CommunicationChannel { get; set; } = Array.Empty<string>();
        public string[] Status { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new SignalStationWarning
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null
                    ? new DateRange()
                    : FixedDateRange.DeepClone() as IDateRange,
                Id = Id ?? "",
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
                CategoryOfSignalStationWarning = CategoryOfSignalStationWarning == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfSignalStationWarning, s => s),
                CommunicationChannel = CommunicationChannel == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CommunicationChannel, s => s),
                Status = Status == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(Status, s => s),
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

            var categoryOfSignalStationWarningNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfSignalStationWarning", mgr);
            if (categoryOfSignalStationWarningNodes != null && categoryOfSignalStationWarningNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfSignalStationWarningNode in categoryOfSignalStationWarningNodes)
                {
                    if (categoryOfSignalStationWarningNode != null && categoryOfSignalStationWarningNode.HasChildNodes)
                    {
                        categories.Add(categoryOfSignalStationWarningNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                CategoryOfSignalStationWarning = categories.ToArray();
            }

            var communicationChannelNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}communicationChannel", mgr);
            if (communicationChannelNodes != null && communicationChannelNodes.Count > 0)
            {
                var communications = new List<string>();
                foreach (XmlNode communicationChannelNode in communicationChannelNodes)
                {
                    if (communicationChannelNode != null && communicationChannelNode.HasChildNodes)
                    {
                        communications.Add(communicationChannelNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                CommunicationChannel = communications.ToArray();
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
