﻿using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class PilotBoardingPlace : GeoFeatureBase, IPilotBoardingPlace, IS127Feature
    {
        public string CallSign { get; set; }
        public string CategoryOfPilotBoardingPlace { get; set; }
        public string CategoryOfPreference { get; set; }
        public string CategoryOfVessel { get; set; }
        public string[] CommunicationChannel { get; set; }
        public string Destination { get; set; }
        public string PilotMovement { get; set; }
        public string PilotVessel { get; set; }
        public string[] Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new PilotBoardingPlace
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
                CallSign = CallSign,
                CategoryOfPilotBoardingPlace = CategoryOfPilotBoardingPlace,
                CategoryOfPreference = CategoryOfPreference,
                CategoryOfVessel = CategoryOfVessel,
                CommunicationChannel = CommunicationChannel == null
                    ? new string[0]
                    : Array.ConvertAll(CommunicationChannel, s => s),
                Destination = Destination,
                PilotMovement = PilotMovement,
                PilotVessel = PilotVessel,
                Status = Status == null
                    ? new string[0]
                    : Array.ConvertAll(Status, s => s),
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

            var callSignNode = node.FirstChild.SelectSingleNode("callSign", mgr);
            if (callSignNode != null && callSignNode.HasChildNodes)
            {
                CallSign = callSignNode.FirstChild.InnerText;
            }

            var categoryOfPilotBoardingPlaceNode = node.FirstChild.SelectSingleNode("categoryOfPilotBoardingPlace", mgr);
            if (categoryOfPilotBoardingPlaceNode != null && categoryOfPilotBoardingPlaceNode.HasChildNodes)
            {
                CategoryOfPilotBoardingPlace = categoryOfPilotBoardingPlaceNode.FirstChild.InnerText;
            }

            var categoryOfPreferenceNode= node.FirstChild.SelectSingleNode("categoryOfPreference", mgr);
            if (categoryOfPreferenceNode != null && categoryOfPreferenceNode.HasChildNodes)
            {
                CategoryOfPilotBoardingPlace = categoryOfPreferenceNode.FirstChild.InnerText;
            }

            var categoryOfVesselNode = node.FirstChild.SelectSingleNode("categoryOfVessel", mgr);
            if (categoryOfVesselNode != null && categoryOfVesselNode.HasChildNodes)
            {
                CategoryOfVessel = categoryOfVesselNode.FirstChild.InnerText;
            }

            var communicationChannelNodes = node.FirstChild.SelectNodes("communicationChannel", mgr);
            if (communicationChannelNodes != null && communicationChannelNodes.Count > 0)
            {
                var channels = new List<string>();
                foreach (XmlNode communicationChannelNode in communicationChannelNodes)
                {
                    if (communicationChannelNode != null && communicationChannelNode.HasChildNodes)
                    {
                        channels.Add(communicationChannelNode.FirstChild.InnerText);
                    }
                }
                CommunicationChannel = channels.ToArray();
            }

            var destinationNode = node.FirstChild.SelectSingleNode("destination", mgr);
            if (destinationNode != null && destinationNode.HasChildNodes)
            {
                Destination = destinationNode.FirstChild.InnerText;
            }

            var pilotMovementNode = node.FirstChild.SelectSingleNode("pilotMovement", mgr);
            if (pilotMovementNode != null && pilotMovementNode.HasChildNodes)
            {
                PilotMovement = pilotMovementNode.FirstChild.InnerText;
            }

            var pilotVesselNode = node.FirstChild.SelectSingleNode("pilotVessel", mgr);
            if (pilotVesselNode != null && pilotVesselNode.HasChildNodes)
            {
                PilotVessel = pilotVesselNode.FirstChild.InnerText;
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
