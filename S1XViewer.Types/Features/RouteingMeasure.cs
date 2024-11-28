using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class RouteingMeasure : GeoFeatureBase, IRouteingMeasure, IS127Feature
    {
        public string CategoryOfRouteingMeasure { get; set; } = string.Empty;
        public string CategoryOfTrafficSeparationScheme { get; set; } = string.Empty;
        public string CategoryOfNavigationLine { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new RouteingMeasure
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
                CategoryOfRouteingMeasure = CategoryOfRouteingMeasure,
                CategoryOfTrafficSeparationScheme = CategoryOfTrafficSeparationScheme,
                CategoryOfNavigationLine = CategoryOfNavigationLine,
                Geometry = Geometry,
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

            var categoryOfRouteingMeasureNode = node.SelectSingleNode("categoryOfRouteingMeasure", mgr);
            if (categoryOfRouteingMeasureNode != null && categoryOfRouteingMeasureNode.HasChildNodes)
            {
                CategoryOfRouteingMeasure = categoryOfRouteingMeasureNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfTrafficSeparationSchemeNode = node.SelectSingleNode("categoryOfTrafficSeparationScheme", mgr);
            if (categoryOfTrafficSeparationSchemeNode != null && categoryOfTrafficSeparationSchemeNode.HasChildNodes)
            {
                CategoryOfTrafficSeparationScheme = categoryOfTrafficSeparationSchemeNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfNavigationLineNode = node.SelectSingleNode("categoryOfNavigationLine", mgr);
            if (categoryOfNavigationLineNode != null && categoryOfNavigationLineNode.HasChildNodes)
            {
                CategoryOfNavigationLine = categoryOfNavigationLineNode.FirstChild?.InnerText ?? string.Empty;
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
