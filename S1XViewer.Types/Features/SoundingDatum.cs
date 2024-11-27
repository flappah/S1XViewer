using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class SoundingDatum : MetaFeatureBase, ISoundingDatum, IS131Feature
    {
        public string VerticalDatum { get; set; } = string.Empty;
        public IInformation[] Information { get; set; } = new Information[0];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new SoundingDatum()
            {
                FeatureObjectIdentifier = FeatureObjectIdentifier == null
                    ? new FeatureObjectIdentifier()
                    : FeatureObjectIdentifier,
                VerticalDatum = VerticalDatum,
                Information = Information == null
                    ? Array.Empty<Information>()
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                Geometry = Geometry,
                Id = Id,
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
                FeatureObjectIdentifier.FromXml(featureObjectIdentifierNode, mgr);
            }

            var foidNode = node.SelectSingleNode("S100:featureObjectIdentifier", mgr);
            if (foidNode != null && foidNode.HasChildNodes)
            {
                FeatureObjectIdentifier = new FeatureObjectIdentifier();
                FeatureObjectIdentifier.FromXml(foidNode, mgr);
            }

            var verticalDatumNode = node.SelectSingleNode("verticalDatum", mgr);
            if (verticalDatumNode != null)
            {
                VerticalDatum = verticalDatumNode.InnerText;
            }

            var informationNodes = node.SelectNodes("information", mgr);
            if (informationNodes != null && informationNodes.Count > 0)
            {
                var informations = new List<Information>();
                foreach (XmlNode informationNode in informationNodes)
                {
                    if (informationNode != null && informationNode.HasChildNodes)
                    {
                        var newInformation = new Information();
                        newInformation.FromXml(informationNode, mgr);
                        informations.Add(newInformation);
                    }
                }
                Information = informations.ToArray();
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
