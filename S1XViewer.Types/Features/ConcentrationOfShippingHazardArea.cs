using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class ConcentrationOfShippingHazardArea : GeoFeatureBase, IConcentrationOfShippingHazardArea, IS127Feature
    {
        public string[] CategoryOfConcentrationOfShippingHazardArea { get; set; } = Array.Empty<string>();
        public string[] Status { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new ConcentrationOfShippingHazardArea
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
                CategoryOfConcentrationOfShippingHazardArea = CategoryOfConcentrationOfShippingHazardArea == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfConcentrationOfShippingHazardArea, s => s),
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

            var categoryOfConcentrationOfShippingHazardAreaNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfConcentrationOfShippingHazardArea", mgr);
            if (categoryOfConcentrationOfShippingHazardAreaNodes != null && categoryOfConcentrationOfShippingHazardAreaNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfConcentrationOfShippingHazardAreaNode in categoryOfConcentrationOfShippingHazardAreaNodes)
                {
                    if (categoryOfConcentrationOfShippingHazardAreaNode != null && categoryOfConcentrationOfShippingHazardAreaNode.HasChildNodes)
                    {
                        categories.Add(categoryOfConcentrationOfShippingHazardAreaNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                categories.Sort();
                CategoryOfConcentrationOfShippingHazardArea = categories.ToArray();
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
