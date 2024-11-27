using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class ServiceHours : InformationFeatureBase, IServiceHours, IS122Feature, IS123Feature, IS127Feature
    {
        public IScheduleByDoW[] ScheduleByDoW { get; set; } = new ScheduleByDoW[0];
        public IInformation[] Information { get; set; } = new Information[0];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new ServiceHours
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null
                    ? new DateRange()
                    : FixedDateRange.DeepClone() as IDateRange,
                Id = Id,
                PeriodicDateRange = PeriodicDateRange == null
                    ? Array.Empty<DateRange>()
                    : Array.ConvertAll(PeriodicDateRange, pdr => pdr.DeepClone() as IDateRange),
                SourceIndication = SourceIndication == null
                    ? Array.Empty<SourceIndication>()
                    : Array.ConvertAll(SourceIndication, si => si.DeepClone() as ISourceIndication),
                ScheduleByDoW = ScheduleByDoW == null 
                    ? Array.Empty<ScheduleByDoW>()
                    : Array.ConvertAll(ScheduleByDoW, sdow => sdow.DeepClone() as IScheduleByDoW),
                Information = Information == null
                    ? Array.Empty<Information>()
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
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

            var informationNodes = node.SelectNodes("information", mgr);
            if (informationNodes != null && informationNodes.Count > 0)
            {
                var informations = new List<Information>();
                foreach (XmlNode informationNode in informationNodes)
                {
                    var newInformation = new Information();
                    newInformation.FromXml(informationNode, mgr);
                    informations.Add(newInformation);
                }
                Information = informations.ToArray();
            }

            var scheduleByDoWNodes = node.SelectNodes("scheduleByDoW", mgr);
            if (scheduleByDoWNodes != null && scheduleByDoWNodes.Count > 0)
            {
                var schedules = new List<ScheduleByDoW>();
                foreach (XmlNode scheduleByDoWNode in scheduleByDoWNodes)
                {
                    if (scheduleByDoWNode != null && scheduleByDoWNode.HasChildNodes)
                    {
                        var scheduleByDoW = new ScheduleByDoW();
                        scheduleByDoW.FromXml(scheduleByDoWNode, mgr);
                        schedules.Add(scheduleByDoW);
                    }
                }
                ScheduleByDoW = schedules.ToArray();
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
