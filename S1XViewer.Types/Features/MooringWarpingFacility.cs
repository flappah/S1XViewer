using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Features
{
    public class MooringWarpingFacility : Layout, IS131Feature, IMooringWarpingFacility
    {
        public string CategoryOfMooringWarpingFacility { get; set; } = string.Empty;
        public string IDCode { get; set; } = string.Empty;
        public string BollardDescription { get; set; } = string.Empty;
        public string BollardPull { get; set; } = string.Empty;
        public bool HeavingLinesFromShore { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new MooringWarpingFacility()
            {
                FeatureName = FeatureName == null
                    ? new IFeatureName[] { new FeatureName() }
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
                    ? new ITextContent[0]
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                Links = Links == null
                    ? new ILink[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink),
                CategoryOfMooringWarpingFacility = CategoryOfMooringWarpingFacility,
                IDCode = IDCode,
                BollardDescription = BollardDescription,
                BollardPull = BollardPull,
                HeavingLinesFromShore = HeavingLinesFromShore
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr);

            var categoryOfMooringWarpingFacilityNode = node.SelectSingleNode("categoryOfMooringWarpingFacility", mgr);
            if (categoryOfMooringWarpingFacilityNode != null && categoryOfMooringWarpingFacilityNode.HasChildNodes)
            {
                CategoryOfMooringWarpingFacility = categoryOfMooringWarpingFacilityNode.FirstChild?.InnerText ?? string.Empty;
            }

            var iDCodeNode = node.SelectSingleNode("iDCode", mgr);
            if (iDCodeNode != null && iDCodeNode.HasChildNodes)
            {
                IDCode = iDCodeNode.FirstChild?.InnerText ?? string.Empty;
            }

            var bollardDescriptionNode = node.SelectSingleNode("bollardDescription", mgr);
            if (bollardDescriptionNode != null && bollardDescriptionNode.HasChildNodes)
            {
                BollardDescription = bollardDescriptionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var bollardPullNode = node.SelectSingleNode("bollardPull", mgr);
            if (bollardPullNode != null && bollardPullNode.HasChildNodes)
            {
                BollardPull = bollardPullNode.FirstChild?.InnerText ?? string.Empty;
            }

            var heavingLinesFromShoreNode = node.SelectSingleNode("heavingLinesFromShore", mgr);
            if (heavingLinesFromShoreNode != null && heavingLinesFromShoreNode.HasChildNodes)
            {
                if (bool.TryParse(heavingLinesFromShoreNode.FirstChild?.InnerText, out bool heavingLinesFromShoreValue))
                {
                    HeavingLinesFromShore = heavingLinesFromShoreValue;
                }
            }

            return this;
        }
    }
}
