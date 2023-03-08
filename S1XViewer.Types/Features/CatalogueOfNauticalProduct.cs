using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
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
                FeatureName = FeatureName == null
                    ? new IFeatureName[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                Graphic = Graphic == null
                    ? new IGraphic[0]
                    : Array.ConvertAll(Graphic, g => g.DeepClone() as IGraphic),
                IssueDate = IssueDate,
                EditionNumber = EditionNumber,
                MarineResourceName = MarineResourceName,
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
                if (node.FirstChild?.Attributes?.Count > 0 &&
                    node.FirstChild?.Attributes.Contains("gml:id") == true)
                {
                    Id = node.FirstChild.Attributes["gml:id"].InnerText;
                }
            }

            var featureNameNodes = node.FirstChild.SelectNodes("featureName", mgr);
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

            var graphicNodes = node.FirstChild.SelectNodes("graphic", mgr);
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

            var issueDateNode = node.FirstChild.SelectSingleNode("issueDate", mgr);
            if (issueDateNode != null && issueDateNode.HasChildNodes)
            {
                IssueDate = issueDateNode.FirstChild.InnerText;
            }

            var editionNumberNode = node.FirstChild.SelectSingleNode("editionNumber", mgr);
            if (editionNumberNode != null && editionNumberNode.HasChildNodes)
            {
                EditionNumber = editionNumberNode.FirstChild.InnerText;
            }

            var marineResourceNameNode = node.FirstChild.SelectSingleNode("marineResourceName", mgr);
            if (marineResourceNameNode != null && marineResourceNameNode.HasChildNodes)
            {
                MarineResourceName = marineResourceNameNode.FirstChild.InnerText;
            }

            var linkNodes = node.FirstChild.SelectNodes("*[boolean(@xlink:href)]", mgr);
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
