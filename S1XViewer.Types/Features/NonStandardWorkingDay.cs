using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Links;
using S1XViewer.Base;

namespace S1XViewer.Types.Features
{
    public class NonStandardWorkingDay : InformationFeatureBase, INonStandardWorkingDay, IS122Feature, IS127Feature
    {
        public string[] DateFixed { get; set; }
        public string[] DateVariable { get; set; }
        public IInformation[] Information { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new NonStandardWorkingDay
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
                DateFixed = DateFixed == null
                    ? new string[0]
                    : Array.ConvertAll(DateFixed, s => s),
                DateVariable = DateVariable == null
                    ? new string[0]
                    : Array.ConvertAll(DateVariable, dv => dv),
                Information = Information == null
                    ? new Information[0]
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                Links = Links == null
                    ? new Link[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
            };
        }

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

            var dateFixedNodes = node.SelectNodes("dateFixed", mgr);
            if (dateFixedNodes != null && dateFixedNodes.Count > 0)
            {
                var datesFixed = new List<string>();
                foreach (XmlNode dateFixedNode in dateFixedNodes)
                {
                    if (dateFixedNode != null && dateFixedNode.HasChildNodes)
                    {
                        datesFixed.Add(dateFixedNode.FirstChild.InnerText);
                    }
                }
                DateFixed = datesFixed.ToArray();
            }

            var dateVariableNodes = node.SelectNodes("dateVariable", mgr);
            if (dateVariableNodes != null && dateVariableNodes.Count > 0)
            {
                var datesVariable = new List<string>();
                foreach (XmlNode dateVariableNode in dateVariableNodes)
                {
                    if (dateVariableNode != null && dateVariableNode.HasChildNodes)
                    {
                        datesVariable.Add(dateVariableNode.FirstChild.InnerText);
                    }
                }
                DateVariable = datesVariable.ToArray();
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
