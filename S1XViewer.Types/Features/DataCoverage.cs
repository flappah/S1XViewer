using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class DataCoverage : MetaFeatureBase, IDataCoverage, IS122Feature, IS123Feature, IS127Feature, IS131Feature
    {
        public string MaximumDisplayScale { get; set; } = string.Empty;
        public string MinimumDisplayScale { get; set; } = string.Empty;

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
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            if (node.HasChildNodes)
            {
                if (node.Attributes?.Count > 0 &&
                    node.Attributes.Contains("gml:id") == true)
                {
                    Id = node.Attributes["gml:id"]?.InnerText ?? string.Empty;
                }
            }

            var featureObjectIdentifierNode = node.SelectSingleNode("S100:featureObjectIdentifier", mgr);
            if (featureObjectIdentifierNode != null && featureObjectIdentifierNode.HasChildNodes)
            {
                FeatureObjectIdentifier = new FeatureObjectIdentifier();
                FeatureObjectIdentifier.FromXml(featureObjectIdentifierNode, mgr, nameSpacePrefix);
            }

            var foidNode = node.SelectSingleNode("S100:featureObjectIdentifier", mgr);
            if (foidNode != null && foidNode.HasChildNodes)
            {
                FeatureObjectIdentifier = new FeatureObjectIdentifier();
                FeatureObjectIdentifier.FromXml(foidNode, mgr, nameSpacePrefix);
            }

            var maximumDisplayScaleNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}maximumDisplayScale", mgr);
            if (maximumDisplayScaleNode != null)
            {
                MaximumDisplayScale = maximumDisplayScaleNode.InnerText;
            }

            var minimumDisplayScaleNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}minimumDisplayScale", mgr);
            if (minimumDisplayScaleNode != null)
            {
                MinimumDisplayScale = minimumDisplayScaleNode.InnerText;
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
