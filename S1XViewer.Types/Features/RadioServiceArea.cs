using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class RadioServiceArea : GeoFeatureBase, IRadioServiceArea, IS123Feature
    {
        public string CallSign { get; set; }
        public string CategoryOfBroadcastCommunication { get; set; }
        public string LanguageInformation { get; set; }
        public IRadioCommunications[] RadioCommunications { get; set; }
        public string Status { get; set; }
        public string TimeReference { get; set; }
        public string TransmissionPower { get; set; }
        public string TxIdentChar { get; set; }
        public string TxTrafficList { get; set; }

        /// <summary>
        ///     Deep clones the object
        /// </summary>
        /// <returns>IComplexType</returns>
        public override IFeature DeepClone()
        {
            return new RadioServiceArea
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
                CategoryOfBroadcastCommunication = CategoryOfBroadcastCommunication,
                LanguageInformation = LanguageInformation,
                RadioCommunications = RadioCommunications == null
                    ? new RadioCommunications[0]
                    : Array.ConvertAll(RadioCommunications, r => r.DeepClone() as IRadioCommunications),
                Status = Status,
                TimeReference = TimeReference,
                TransmissionPower = TransmissionPower,
                TxIdentChar = TxIdentChar,
                TxTrafficList = TxTrafficList,
                Links = Links == null
                    ? new Link[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
            };
        }

        /// <summary>
        ///     Reads the data from an XML dom
        /// </summary>
        /// <param name="node">current node to use as a starting point for reading</param>
        /// <param name="mgr">xml namespace manager</param>
        /// <returns>IFeature</returns>
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node != null && node.HasChildNodes)
            {
                if (node.FirstChild.Attributes.Count > 0)
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

            var categoryOfBroadcastCommunicationNode = node.FirstChild.SelectSingleNode("categoryOfBroadcastCommunication", mgr);
            if (categoryOfBroadcastCommunicationNode != null && categoryOfBroadcastCommunicationNode.HasChildNodes)
            {
                CategoryOfBroadcastCommunication = categoryOfBroadcastCommunicationNode.FirstChild.InnerText;
            }

            var languageInformationNode = node.FirstChild.SelectSingleNode("languageInformation", mgr);
            if (languageInformationNode != null && languageInformationNode.HasChildNodes)
            {
                LanguageInformation = languageInformationNode.FirstChild.InnerText;
            }

            var radioCommunicationNodes = node.FirstChild.SelectNodes("radiocommunications", mgr);
            if (radioCommunicationNodes != null && radioCommunicationNodes.Count > 0)
            {
                var radioCommunications = new List<RadioCommunications>();
                foreach (XmlNode radioCommunicationNode in radioCommunicationNodes)
                {
                    if (radioCommunicationNode != null && radioCommunicationNode.HasChildNodes)
                    {
                        var newRadioCommunications = new RadioCommunications();
                        newRadioCommunications.FromXml(radioCommunicationNode, mgr);
                        radioCommunications.Add(newRadioCommunications);
                    }
                }
                RadioCommunications = radioCommunications.ToArray();
            }

            var statusNode = node.FirstChild.SelectSingleNode("status", mgr);
            if (statusNode != null && statusNode.HasChildNodes)
            {
                Status = statusNode.FirstChild.InnerText;
            }

            var timeReferenceNode = node.FirstChild.SelectSingleNode("timeReference", mgr);
            if (timeReferenceNode != null && timeReferenceNode.HasChildNodes)
            {
                TimeReference = timeReferenceNode.FirstChild.InnerText;
            }

            var transmissionPowerNode = node.FirstChild.SelectSingleNode("transmissionPower", mgr);
            if (transmissionPowerNode != null && transmissionPowerNode.HasChildNodes)
            {
                TransmissionPower = transmissionPowerNode.FirstChild.InnerText;
            }

            var txIdentCharNode = node.FirstChild.SelectSingleNode("txIdentChar", mgr);
            if (txIdentCharNode != null && txIdentCharNode.HasChildNodes)
            {
                TxIdentChar = txIdentCharNode.FirstChild.InnerText;
            }

            var txTrafficListNode = node.FirstChild.SelectSingleNode("txTrafficList", mgr);
            if (txTrafficListNode != null && txTrafficListNode.HasChildNodes)
            {
                TxTrafficList = txTrafficListNode.FirstChild.InnerText;
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
