using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class HarbourFacility : HarbourPhysicalInfrastructure, IS131Feature, IHarbourFacility
    {
        public string[] CategoryOfHarbourFacility { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new HarbourFacility()
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
                Links = Links == null
                    ? Array.Empty<Link>()
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink),
                VerticalClearance = VerticalClearance,
                CategoryOfHarbourFacility = CategoryOfHarbourFacility == null ? new string[0] : Array.ConvertAll(CategoryOfHarbourFacility, chf => chf)
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

            return this;
        }
    }
}
