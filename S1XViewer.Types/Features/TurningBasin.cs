using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class TurningBasin : Layout, IS131Feature, ITurningBasin
    {
        public IDepthsDescription DepthsDescription { get; set; } = new DepthsDescription();
        public string LocationByText { get; set; } = string.Empty;
        public IMarkedBy MarkedBy { get; set; } = new MarkedBy();
        public string ISPSLevel { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new TurningBasin()
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
            return "";
        }
    }
}
