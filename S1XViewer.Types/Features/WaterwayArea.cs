using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class WaterwayArea : Layout, IWaterwayArea, IS127Feature, IS131Feature
    {
        public string DynamicResource { get; set; } = string.Empty;
        public string SiltationRate { get; set; } = string.Empty;
        public string[] Status { get; set; } = Array.Empty<string>();

        //S131
        public string CategoryOfPortSection { get; set; } = string.Empty;
        public IDepthsDescription DepthsDescription { get; set; } = new DepthsDescription();
        public string LocationByText { get; set; } = string.Empty;
        public IMarkedBy MarkedBy { get; set; } = new MarkedBy();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new WaterwayArea
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
                DynamicResource = DynamicResource,
                SiltationRate = SiltationRate,
                Status = Status == null
                    ? new string[0]
                    : Array.ConvertAll(Status, s => s),
                CategoryOfPortSection = CategoryOfPortSection,
                DepthsDescription = DepthsDescription == null ? new DepthsDescription() : DepthsDescription.DeepClone() as IDepthsDescription,
                LocationByText = LocationByText,
                MarkedBy = MarkedBy == null ? new MarkedBy() : MarkedBy.DeepClone() as IMarkedBy
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

            var dynamicResourceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}dynamicResource", mgr);
            if (dynamicResourceNode != null)
            {
                DynamicResource = dynamicResourceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var siltationRateNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}siltationRate", mgr);
            if (siltationRateNode != null)
            {
                SiltationRate = siltationRateNode.FirstChild?.InnerText ?? string.Empty;
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
                Status = statuses.ToArray();
            }

            var categoryOfPortSectionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfPortSection", mgr);
            if (categoryOfPortSectionNode != null)
            {
                CategoryOfPortSection = categoryOfPortSectionNode.FirstChild?.InnerText ?? string.Empty;
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
