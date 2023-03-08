using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class DataCoverage : MetaFeatureBase, IDataCoverage, IS122Feature, IS123Feature, IS127Feature
    {
        public string MaximumDisplayScale { get; set; }
        public string MinimumDisplayScale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new DataCoverage()
            {
                FeatureObjectIdentifier = FeatureObjectIdentifier == null 
                    ? new FeatureObjectIdentifier()
                    : FeatureObjectIdentifier,
                MaximumDisplayScale = MaximumDisplayScale,
                MinimumDisplayScale = MinimumDisplayScale,
                Geometry = Geometry,
                Id = Id,
                Links = Links == null
                    ? new Link[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
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

            var featureObjectIdentifierNode = node.FirstChild.SelectSingleNode("s100:featureObjectIdentifier", mgr);
            if (featureObjectIdentifierNode != null && featureObjectIdentifierNode.HasChildNodes)
            {
                FeatureObjectIdentifier = new FeatureObjectIdentifier();
                FeatureObjectIdentifier.FromXml(featureObjectIdentifierNode, mgr);
            }

            var foidNode = node.FirstChild.SelectSingleNode("s100:featureObjectIdentifier", mgr);
            if (foidNode != null && foidNode.HasChildNodes)
            {
                FeatureObjectIdentifier = new FeatureObjectIdentifier();
                FeatureObjectIdentifier.FromXml(foidNode, mgr);
            }

            var maximumDisplayScaleNode = node.FirstChild.SelectSingleNode("maximumDisplayScale", mgr);
            if (maximumDisplayScaleNode != null)
            {
                MaximumDisplayScale = maximumDisplayScaleNode.InnerText;
            }

            var minimumDisplayScaleNode = node.FirstChild.SelectSingleNode("minimumDisplayScale", mgr);
            if (minimumDisplayScaleNode != null)
            {
                MinimumDisplayScale = minimumDisplayScaleNode.InnerText;
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
