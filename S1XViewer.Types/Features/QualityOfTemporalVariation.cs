using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class QualityOfTemporalVariation : DataQuality, IQualityOfTemporalVariation, IS127Feature
    {
        public string CategoryOfTemporalVariation { get; set; } = string.Empty;

        /// <summary>
        ///     Deep clones the object
        /// </summary>
        /// <returns>IComplexType</returns>
        public override IFeature DeepClone()
        {
            return new QualityOfTemporalVariation
            {
                Information = Information == null 
                    ? new IInformation[0]
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                CategoryOfTemporalVariation = CategoryOfTemporalVariation,
                FeatureObjectIdentifier = FeatureObjectIdentifier == null
                    ? new FeatureObjectIdentifier()
                    : FeatureObjectIdentifier.DeepClone() as IFeatureObjectIdentifier,
                Geometry = Geometry,
                Id = Id,
                Links = Links == null
                    ? new ILink[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
            };
        }

        /// <summary>
        ///     Reads the data from an XML dom
        /// </summary>
        /// <param name="node">current node to use as a starting point for reading</param>
        /// <param name="mgr">xml namespace manager</param>
        /// <returns>IFeature</returns>
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

            var categoryOfTemporalVariation = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfTemporalVariation", mgr);
            if (categoryOfTemporalVariation != null)
            {
                CategoryOfTemporalVariation = categoryOfTemporalVariation.InnerText;
            }

            var informationNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}information", mgr);
            if (informationNodes != null && informationNodes.Count > 0)
            {
                var informationItems = new List<Information>();
                foreach (XmlNode informationNode in informationNodes)
                {
                    if (informationNode != null && informationNode.HasChildNodes)
                    {
                        var newInformation = new Information();
                        newInformation.FromXml(informationNode, mgr, nameSpacePrefix);
                        informationItems.Add(newInformation);
                    }
                }
                Information = informationItems.ToArray();
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
