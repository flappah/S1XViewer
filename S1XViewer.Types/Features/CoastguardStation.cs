using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class CoastguardStation : GeoFeatureBase, ICoastguardStation, IS123Feature
    {
        public string[] CommunicationsChannel { get; set; } = Array.Empty<string>();
        public string IsMRCC { get; set; } = string.Empty;
        public string[] Status { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new CoastguardStation
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
                CommunicationsChannel = CommunicationsChannel == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CommunicationsChannel, f => f),
                IsMRCC = IsMRCC,
                Status = Status == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(Status, f => f),
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
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr);

            var communicationsChannelNodes = node.SelectNodes("communicationsChannel", mgr);
            if (communicationsChannelNodes != null && communicationsChannelNodes.Count > 0)
            {
                var communications = new List<string>();
                foreach (XmlNode communicationsChannelNode in communicationsChannelNodes)
                {
                    if (communicationsChannelNode != null && communicationsChannelNode.HasChildNodes)
                    {
                        var communication = communicationsChannelNode.FirstChild?.InnerText ?? string.Empty;
                        communications.Add(communication);
                    }
                }

                communications.Sort();
                CommunicationsChannel = communications.ToArray();
            }

            var ismrccNode = node.SelectSingleNode("isMRCC", mgr);
            if (ismrccNode != null && ismrccNode.HasChildNodes)
            {
                IsMRCC = ismrccNode.FirstChild?.InnerText ?? string.Empty;
            }

            var statusNodes = node.SelectNodes("status", mgr);
            if (statusNodes != null && statusNodes.Count > 0)
            {
                var statuses = new List<string>();
                foreach (XmlNode statusNode in statusNodes)
                {
                    if (statusNode != null && statusNode.HasChildNodes)
                    {
                        var status = statusNode.FirstChild?.InnerText ?? string.Empty;
                        statuses.Add(status);
                    }
                }

                statuses.Sort();
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
