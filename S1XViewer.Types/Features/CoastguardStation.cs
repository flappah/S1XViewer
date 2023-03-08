using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class CoastguardStation : GeoFeatureBase, ICoastguardStation, IS123Feature
    {
        public string[] CommunicationsChannel { get; set; }
        public string IsMRCC { get; set; }
        public string[] Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new CoastguardStation
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
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                TextContent = TextContent == null
                    ? new TextContent[0]
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                CommunicationsChannel = CommunicationsChannel == null
                    ? new string[0]
                    : Array.ConvertAll(CommunicationsChannel, f => f),
                IsMRCC = IsMRCC,
                Status = Status == null
                    ? new string[0]
                    : Array.ConvertAll(Status, f => f),
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
                if (node.FirstChild?.Attributes?.Count > 0 &&
                    node.FirstChild?.Attributes.Contains("gml:id") == true)
                {
                    Id = node.FirstChild.Attributes["gml:id"].InnerText;
                }
            }

            var periodicDateRangeNodes = node.FirstChild.SelectNodes("periodicDateRange", mgr);
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

            var fixedDateRangeNode = node.FirstChild.SelectSingleNode("fixedDateRange", mgr);
            if (fixedDateRangeNode != null && fixedDateRangeNode.HasChildNodes)
            {
                FixedDateRange = new DateRange();
                FixedDateRange.FromXml(fixedDateRangeNode, mgr);
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

            var sourceIndication = node.FirstChild.SelectSingleNode("sourceIndication", mgr);
            if (sourceIndication != null && sourceIndication.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndication, mgr);
            }

            var textContentNodes = node.FirstChild.SelectNodes("textContent", mgr);
            if (textContentNodes != null && textContentNodes.Count > 0)
            {
                var textContents = new List<TextContent>();
                foreach (XmlNode textContentNode in textContentNodes)
                {
                    if (textContentNode != null && textContentNode.HasChildNodes)
                    {
                        var content = new TextContent();
                        content.FromXml(textContentNode, mgr);
                        textContents.Add(content);
                    }
                }
                TextContent = textContents.ToArray();
            }

            var communicationsChannelNodes = node.FirstChild.SelectNodes("communicationsChannel", mgr);
            if (communicationsChannelNodes != null && communicationsChannelNodes.Count > 0)
            {
                var communications = new List<string>();
                foreach (XmlNode communicationsChannelNode in communicationsChannelNodes)
                {
                    if (communicationsChannelNode != null && communicationsChannelNode.HasChildNodes)
                    {
                        var communication = communicationsChannelNode.FirstChild.InnerText;
                        communications.Add(communication);
                    }
                }

                communications.Sort();
                CommunicationsChannel = communications.ToArray();
            }

            var ismrccNode = node.FirstChild.SelectSingleNode("isMRCC", mgr);
            if (ismrccNode != null && ismrccNode.HasChildNodes)
            {
                IsMRCC = ismrccNode.FirstChild.InnerText;
            }

            var statusNodes = node.FirstChild.SelectNodes("status", mgr);
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

                statuses.Sort();
                Status = statuses.ToArray();
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
