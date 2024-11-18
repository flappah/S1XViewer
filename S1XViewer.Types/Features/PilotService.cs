﻿using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class PilotService : GeoFeatureBase, IPilotService, IS127Feature
    {
        public string[] CategoryOfPilot { get; set; }
        public string PilotQualification { get; set; }
        public string PilotRequest { get; set; }
        public string RemotePilot { get; set; }
        public INoticeTime NoticeTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new PilotService
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
                CategoryOfPilot = CategoryOfPilot == null
                    ? new string[0]
                    : Array.ConvertAll(CategoryOfPilot, s => s),
                PilotQualification = PilotQualification,
                PilotRequest = PilotRequest,
                RemotePilot = RemotePilot,
                NoticeTime = NoticeTime == null
                    ? new NoticeTime()
                    : NoticeTime.DeepClone() as INoticeTime,
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

            var fixedDateRangeNode = node.SelectSingleNode("fixedDateRange", mgr);
            if (fixedDateRangeNode != null && fixedDateRangeNode.HasChildNodes)
            {
                FixedDateRange = new DateRange();
                FixedDateRange.FromXml(fixedDateRangeNode, mgr);
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

            var sourceIndication = node.SelectSingleNode("sourceIndication", mgr);
            if (sourceIndication != null && sourceIndication.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndication, mgr);
            }

            var textContentNodes = node.SelectNodes("textContent", mgr);
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

            var categoryOfPilotNodes = node.SelectNodes("categoryOfPilot", mgr);
            if (categoryOfPilotNodes != null && categoryOfPilotNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfPilotNode in categoryOfPilotNodes)
                {
                    var category = categoryOfPilotNode.FirstChild.InnerText;
                    categories.Add(category);
                }
                CategoryOfPilot = categories.ToArray();
            }

            var pilotQualificationNode = node.SelectSingleNode("pilotQualification", mgr);
            if (pilotQualificationNode != null && pilotQualificationNode.HasChildNodes)
            {
                PilotQualification = pilotQualificationNode.FirstChild.InnerText;
            }

            var pilotRequestNode = node.SelectSingleNode("pilotRequest", mgr);
            if (pilotRequestNode != null && pilotRequestNode.HasChildNodes)
            {
                PilotRequest = pilotRequestNode.FirstChild.InnerText;
            }

            var remotePilotNode = node.SelectSingleNode("remotePilot", mgr);
            if (remotePilotNode != null && remotePilotNode.HasChildNodes)
            {
                RemotePilot = remotePilotNode.FirstChild.InnerText;
            }

            var noticeTimeNode = node.SelectSingleNode("noticeTime", mgr);
            if (noticeTimeNode != null && noticeTimeNode.HasChildNodes)
            {
                NoticeTime = new NoticeTime();
                NoticeTime.FromXml(noticeTimeNode, mgr);
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
