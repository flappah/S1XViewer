using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class HarbourAreaSection : Layout, IS131Feature, IHarbourAreaSection
    {
        public string CategoryOfPortSection { get; set; } = string.Empty;
        public string[] CategoryOfHarbourFacility { get; set; } = Array.Empty<string>();
        public string iSPSLevel { get; set; } = string.Empty;
        public string FacilitiesLayoutDescription { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new HarbourAreaSection()
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
                    ? new ILink[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink),
                CategoryOfPortSection = CategoryOfPortSection,
                CategoryOfHarbourFacility = CategoryOfHarbourFacility == null ? new string[0] : CategoryOfHarbourFacility,
                iSPSLevel = iSPSLevel,
                FacilitiesLayoutDescription = FacilitiesLayoutDescription
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

            var categoryOfPortSectionNode = node.SelectSingleNode("categoryOfPortSection", mgr);
            if (categoryOfPortSectionNode != null && categoryOfPortSectionNode.HasChildNodes)
            {
                CategoryOfPortSection = categoryOfPortSectionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfHarbourFacilityNodes = node.SelectNodes("categoryOfHarbourFacility", mgr);
            if (categoryOfHarbourFacilityNodes != null && categoryOfHarbourFacilityNodes.Count > 0)
            {
                var harbourFacilities = new List<string>();
                foreach (XmlNode categoryOfHarbourFacilityNode in categoryOfHarbourFacilityNodes)
                {
                    if (categoryOfHarbourFacilityNode != null && categoryOfHarbourFacilityNode.HasChildNodes && String.IsNullOrEmpty(categoryOfHarbourFacilityNode.FirstChild?.InnerText) == false)
                    {
                        harbourFacilities.Add(categoryOfHarbourFacilityNode.FirstChild?.InnerText);
                    }
                }

                harbourFacilities.Sort();
                CategoryOfHarbourFacility = harbourFacilities.ToArray();
            }

            var iSPSLevelNode = node.SelectSingleNode("iSPSLevel", mgr);
            if (iSPSLevelNode != null && iSPSLevelNode.HasChildNodes)
            {
                iSPSLevel = iSPSLevelNode.FirstChild?.InnerText ?? string.Empty;
            }

            var facilitiesLayoutDescriptionNode = node.SelectSingleNode("facilitiesLayoutDescription", mgr);
            if (facilitiesLayoutDescriptionNode != null && facilitiesLayoutDescriptionNode.HasChildNodes)
            {
                FacilitiesLayoutDescription = facilitiesLayoutDescriptionNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
