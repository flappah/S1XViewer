using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Regulations : AbstractRxn, IRegulations, IS122Feature, IS123Feature, IS127Feature
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new Regulations
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
                    ? new SourceIndication[0]
                    : Array.ConvertAll(SourceIndication, s => s.DeepClone() as ISourceIndication),
                CategoryOfAuthority = CategoryOfAuthority ?? "",
                Graphic = Graphic == null
                    ? new Graphic[0]
                    : Array.ConvertAll(Graphic, g => g.DeepClone() as IGraphic),
                RxnCode = RxnCode == null
                    ? new RxnCode[0]
                    : Array.ConvertAll(RxnCode, r => r.DeepClone() as IRxnCode),
                TextContent = TextContent == null
                    ? new TextContent[0]
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Links = Links == null
                    ? new Link[0]
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
                    Id = node.Attributes["gml:id"].InnerText;
                }
            }

            var fixedDateRangeNode = node.SelectSingleNode("fixedDateRange", mgr);
            if (fixedDateRangeNode != null && fixedDateRangeNode.HasChildNodes)
            {
                FixedDateRange = new DateRange();
                FixedDateRange.FromXml(fixedDateRangeNode, mgr);
            }

            var periodicDateRangeNodes = node.SelectNodes("periodicDateRange", mgr);
            if (periodicDateRangeNodes != null && periodicDateRangeNodes.Count > 0)
            {
                var dateRanges = new List<DateRange>();
                foreach (XmlNode periodicDateRangeNode in periodicDateRangeNodes)
                {
                    var newDateRange = new DateRange();
                    newDateRange.FromXml(periodicDateRangeNode, mgr);
                    dateRanges.Add(newDateRange);
                }
                PeriodicDateRange = dateRanges.ToArray();
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

            var sourceIndicationNodes = node.SelectNodes("sourceIndication", mgr);
            if (sourceIndicationNodes != null && sourceIndicationNodes.Count > 0)
            {
                var sourceIndications = new List<SourceIndication>();
                foreach (XmlNode sourceIndicationNode in sourceIndicationNodes)
                {
                    if (sourceIndicationNode != null && sourceIndicationNode.HasChildNodes)
                    {
                        var sourceIndication = new SourceIndication();
                        sourceIndication.FromXml(sourceIndicationNode, mgr);
                        sourceIndications.Add(sourceIndication);
                    }
                }
                SourceIndication = sourceIndications.ToArray();
            }

            var textContentNodes = node.SelectNodes("textContent", mgr);
            if (textContentNodes != null && textContentNodes.Count > 0)
            {
                var textContents = new List<TextContent>();
                foreach (XmlNode textContentNode in textContentNodes)
                {
                    if (textContentNode != null && textContentNode.HasChildNodes)
                    {
                        var newTextContent = new TextContent();
                        newTextContent.FromXml(textContentNode, mgr);
                        textContents.Add(newTextContent);
                    }
                }

                TextContent = textContents.ToArray();
            }

            var categoryOfAuthorityNode = node.SelectSingleNode("categoryOfAuthority", mgr);
            if (categoryOfAuthorityNode != null && categoryOfAuthorityNode.HasChildNodes)
            {
                CategoryOfAuthority = categoryOfAuthorityNode.FirstChild.InnerText;
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

            var rxnCodeNodes = node.SelectNodes("rxnCode");
            if (rxnCodeNodes != null && rxnCodeNodes.Count > 0)
            {
                var rxnCodes = new List<RxnCode>();
                foreach (XmlNode rxnCodeNode in rxnCodeNodes)
                {
                    if (rxnCodeNode != null && rxnCodeNode.HasChildNodes)
                    {
                        var newRxnCode = new RxnCode();
                        newRxnCode.FromXml(rxnCodeNode, mgr);
                        rxnCodes.Add(newRxnCode);
                    }
                }
                RxnCode = rxnCodes.ToArray();
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
