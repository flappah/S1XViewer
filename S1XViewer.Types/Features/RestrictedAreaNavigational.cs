﻿using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    [Serializable]
    public class RestrictedAreaNavigational : GeoFeatureBase, IRestrictedAreaNavigational, IS122Feature, IS127Feature
    {
        public string[] CategoryOfRestrictedArea { get; set; } = Array.Empty<string>();
        public string[] Restriction { get; set; } = Array.Empty<string>();
        public string[] Status { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new RestrictedAreaNavigational
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
                CategoryOfRestrictedArea = CategoryOfRestrictedArea == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfRestrictedArea, s => s),
                Restriction = Restriction == null ? Array.Empty<string>() : Array.ConvertAll(Restriction, s => s),
                Status = Status == null 
                    ? Array.Empty<string>()
                    : Array.ConvertAll(Status, s => s),
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
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr, nameSpacePrefix);

            var categoryOfRestrictedAreaNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfRestrictedArea", mgr);
            if (categoryOfRestrictedAreaNodes != null && categoryOfRestrictedAreaNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfRestrictedAreaNode in categoryOfRestrictedAreaNodes)
                {
                    if (categoryOfRestrictedAreaNode != null && categoryOfRestrictedAreaNode.HasChildNodes)
                    {
                        categories.Add(categoryOfRestrictedAreaNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                CategoryOfRestrictedArea = categories.ToArray();
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
                        statuses.Add(statusNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
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
