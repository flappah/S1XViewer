using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Landmark : GeoFeatureBase, ILandmark, IS123Feature
    {
        public string[] CategoryOfLandmark { get; set; } = Array.Empty<string>();
        public string[] Function { get; set; } = Array.Empty<string>();
        public string[] Status { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new Landmark
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
                CategoryOfLandmark = CategoryOfLandmark == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfLandmark, s => s),
                Function = Function == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(Function, s => s),
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

            var categoryOfLandmarkNodes = node.SelectNodes("categoryOfLandmark", mgr);
            if (categoryOfLandmarkNodes != null && categoryOfLandmarkNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfLandmarkNode in categoryOfLandmarkNodes)
                {
                    if (categoryOfLandmarkNode != null && categoryOfLandmarkNode.HasChildNodes)
                    {
                        var category = categoryOfLandmarkNode.FirstChild.InnerText;
                        categories.Add(category);
                    }
                }
                CategoryOfLandmark = categories.ToArray();
            }

            var functionNodes = node.SelectNodes("function", mgr);
            if (functionNodes != null && functionNodes.Count > 0)
            {
                var functions = new List<string>();
                foreach (XmlNode functionNode in functionNodes)
                {
                    if (functionNode != null && functionNode.HasChildNodes)
                    {
                        var function = functionNode.FirstChild.InnerText;
                        functions.Add(function);
                    }
                }
                Function = functions.ToArray();
            }

            var statusNodes = node.SelectNodes("status", mgr);
            if (statusNodes != null && statusNodes.Count > 0)
            {
                var statuses = new List<string>();
                foreach (XmlNode statusNode in statusNodes)
                {
                    if (statusNode != null && statusNode.HasChildNodes)
                    {
                        var status = statusNode.FirstChild.InnerText;
                        statuses.Add(status);
                    }
                }
                Status = statuses.ToArray();
            }

            return this;
        }
    }
}
