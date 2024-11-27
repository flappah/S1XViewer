using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class MilitaryPracticeArea : GeoFeatureBase, IMilitaryPracticeArea, IS127Feature
    {
        public string[] CategoryOfMilitaryPracticeArea { get; set; }
        public string Nationality { get; set; }
        public string[] Restriction { get; set; }
        public string[] Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new MilitaryPracticeArea
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FeatureObjectIdentifier = FeatureObjectIdentifier == null
                    ? new FeatureObjectIdentifier()
                    : FeatureObjectIdentifier.DeepClone() as IFeatureObjectIdentifier,
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
                CategoryOfMilitaryPracticeArea = CategoryOfMilitaryPracticeArea == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfMilitaryPracticeArea, s => s),
                Nationality = Nationality,
                Restriction = Restriction == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(Restriction, s => s),
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
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr);

            var categoryOfMilitaryPracticeAreaNodes = node.SelectNodes("categoryOfMilitaryPracticeArea", mgr);
            if (categoryOfMilitaryPracticeAreaNodes != null && categoryOfMilitaryPracticeAreaNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfMilitaryPracticeAreaNode in categoryOfMilitaryPracticeAreaNodes)
                {
                    if (categoryOfMilitaryPracticeAreaNode != null && categoryOfMilitaryPracticeAreaNode.HasChildNodes)
                    {
                        categories.Add(categoryOfMilitaryPracticeAreaNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                categories.Sort();
                CategoryOfMilitaryPracticeArea = categories.ToArray();
            }

            var nationalityNode = node.SelectSingleNode("nationality", mgr);
            if (nationalityNode != null && nationalityNode.HasChildNodes)
            {
                Nationality = nationalityNode.FirstChild?.InnerText ?? string.Empty;
            }

            var restrictionNodes = node.SelectNodes("restriction", mgr);
            if (restrictionNodes != null && restrictionNodes.Count > 0)
            {
                var restrictions = new List<string>();
                foreach (XmlNode restrictionNode in restrictionNodes)
                {
                    if (restrictionNode != null && restrictionNode.HasChildNodes)
                    {
                        restrictions.Add(restrictionNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                restrictions.Sort();
                Restriction = restrictions.ToArray();
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
    }
}
