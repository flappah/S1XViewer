using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class GMDSSArea : GeoFeatureBase, IGMDSSArea, IS123Feature
    {
        public string[] CategoryOfGMDSSArea { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new GMDSSArea
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
                    ? Array.Empty<TextContent>()
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                CategoryOfGMDSSArea = CategoryOfGMDSSArea == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfGMDSSArea, s => s),
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

            var categoryOfGMDSSAreaNodes = node.SelectNodes("categoryOfGMDSSArea", mgr);
            if (categoryOfGMDSSAreaNodes != null && categoryOfGMDSSAreaNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfGMDSSAreaNode in categoryOfGMDSSAreaNodes)
                {
                    if (categoryOfGMDSSAreaNode != null && categoryOfGMDSSAreaNode.HasChildNodes)
                    {
                        var category = categoryOfGMDSSAreaNode.FirstChild.InnerText;
                        categories.Add(category);
                    }
                }
                CategoryOfGMDSSArea = categories.ToArray();
            }

            return this;
        }
    }
}
