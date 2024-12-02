using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class MilitaryPracticeArea : GeoFeatureBase, IMilitaryPracticeArea, IS127Feature
    {
        public string[] CategoryOfMilitaryPracticeArea { get; set; } = Array.Empty<string>();
        public string Nationality { get; set; } = string.Empty;
        public string[] Restriction { get; set; } = Array.Empty<string>();
        public string[] Status { get; set; } = Array.Empty<string>();

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
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr, nameSpacePrefix);

            var categoryOfMilitaryPracticeAreaNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfMilitaryPracticeArea", mgr);
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

            var nationalityNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}nationality", mgr);
            if (nationalityNode != null && nationalityNode.HasChildNodes)
            {
                Nationality = nationalityNode.FirstChild?.InnerText ?? string.Empty;
            }

            var restrictionNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}restriction", mgr);
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

            var statusNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}status", mgr);
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
