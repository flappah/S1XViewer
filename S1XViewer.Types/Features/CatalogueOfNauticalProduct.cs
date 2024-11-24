﻿using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class CatalogueOfNauticalProduct : GeoFeatureBase, ICatalogueOfNauticalProduct, IS128Feature
    {
        public string EditionNumber { get; set; }
        public string IssueDate { get; set; }
        public string MarineResourceName { get; set; }
        public IGraphic[] Graphic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new CatalogueOfNauticalProduct
            {
                Id = Id,
                Geometry = Geometry,
                FeatureName = FeatureName == null
                    ? new IFeatureName[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null ? new DateRange() : FixedDateRange.DeepClone() as IDateRange,
                PeriodicDateRange = PeriodicDateRange == null ? new DateRange[0] : Array.ConvertAll(PeriodicDateRange, pdr => pdr as IDateRange),
                SourceIndication = SourceIndication == null
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                TextContent = TextContent == null ? new TextContent[0] : Array.ConvertAll(TextContent, tc => tc.DeepClone() as ITextContent),
                EditionNumber = EditionNumber,
                IssueDate = IssueDate,
                MarineResourceName = MarineResourceName,
                Graphic = Graphic == null
                    ? new IGraphic[0]
                    : Array.ConvertAll(Graphic, g => g.DeepClone() as IGraphic),
                Links = Links == null
                    ? new ILink[0]
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

            if (node.HasChildNodes)
            {
                if (node.Attributes?.Count > 0 && node.Attributes.Contains("gml:id") == true)
                {
                    Id = node.Attributes["gml:id"]?.InnerText ?? string.Empty;
                }
            }

            var featureNameNodes = node.SelectNodes("featureName", mgr);
            if (featureNameNodes != null && featureNameNodes.Count > 0)
            {
                var featureNames = new List<FeatureName>();
                foreach (XmlNode featureNameNode in featureNameNodes)
                {
                    var newFeatureName = new FeatureName();
                    newFeatureName.FromXml(featureNameNode, mgr);
                    featureNames.Add(newFeatureName);
                }
                FeatureName = featureNames.ToArray();
            }

            var graphicNodes = node.SelectNodes("graphic", mgr);
            if (graphicNodes != null && graphicNodes.Count > 0)
            {
                var graphics = new List<Graphic>();
                foreach (XmlNode graphicNode in graphicNodes)
                {
                    if (graphicNode != null && graphicNode.HasChildNodes)
                    {
                        var newGraphic = new Graphic();
                        newGraphic.FromXml(graphicNode, mgr);
                        graphics.Add(newGraphic);
                    }
                }
                Graphic = graphics.ToArray();
            }

            var issueDateNode = node.SelectSingleNode("issueDate", mgr);
            if (issueDateNode != null && issueDateNode.HasChildNodes)
            {
                IssueDate = issueDateNode.InnerText;
            }

            var editionNumberNode = node.SelectSingleNode("editionNumber", mgr);
            if (editionNumberNode != null && editionNumberNode.HasChildNodes)
            {
                EditionNumber = editionNumberNode.InnerText;
            }

            var marineResourceNameNode = node.SelectSingleNode("marineResourceName", mgr);
            if (marineResourceNameNode != null && marineResourceNameNode.HasChildNodes)
            {
                MarineResourceName = marineResourceNameNode.InnerText;
            }

            var linkNodes = node.SelectNodes("*[boolean(@xlink:href)]", mgr);
            if (linkNodes != null && linkNodes.Count > 0)
            {
                var links = new List<Link>();
                foreach (XmlNode linkNode in linkNodes)
                {
                    var newLink = new Link();
                    newLink.FromXml(linkNode, mgr);
                    links.Add(newLink);
                }
                Links = links.ToArray();
            }

            return this;
        }
    }
}
