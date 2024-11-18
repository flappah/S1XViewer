using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Applicability : InformationFeatureBase, IApplicability, IS122Feature, IS123Feature, IS127Feature, IFeature
    {
        public string Ballast { get; set; }
        public string[] CategoryOfCargo { get; set; }
        public string[] CategoryOfDangerousOrHazardousCargo { get; set; }
        public string CategoryOfVessel { get; set; }
        public string CategoryOfVesselRegistry { get; set; }
        public string LogicalConnectives { get; set; }
        public string ThicknessOfIceCapability { get; set; }
        public IVesselsMeasurement[] VesselsMeasurements { get; set; }
        public IInformation[] Information { get; set; }
        public string VesselPerformance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new Applicability
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
                Ballast = Ballast,
                CategoryOfCargo = CategoryOfCargo == null 
                    ? new string[0]
                    : Array.ConvertAll(CategoryOfCargo, s => s),
                CategoryOfDangerousOrHazardousCargo = CategoryOfDangerousOrHazardousCargo == null
                    ? new string[0]
                    : Array.ConvertAll(CategoryOfDangerousOrHazardousCargo, s => s),
                CategoryOfVessel = CategoryOfVessel,
                CategoryOfVesselRegistry = CategoryOfVesselRegistry,
                Information = Information == null
                    ? new Information[0]
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                LogicalConnectives = LogicalConnectives,
                ThicknessOfIceCapability = ThicknessOfIceCapability,
                VesselsMeasurements = VesselsMeasurements == null
                    ? new VesselsMeasurement[0]
                    : Array.ConvertAll(VesselsMeasurements, v => v.DeepClone() as IVesselsMeasurement),
                VesselPerformance = VesselPerformance,
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

            var ballastNode = node.SelectSingleNode("ballast", mgr);
            if (ballastNode != null && ballastNode.HasChildNodes)
            {
                Ballast = ballastNode.FirstChild.InnerText;
            }

            var categoryOfCargoNodes = node.SelectNodes("categoryOfCargo", mgr);
            if (categoryOfCargoNodes != null && categoryOfCargoNodes.Count > 0)
            {
                var categoriesOfCargo = new List<string>();
                foreach(XmlNode categoryOfCargoNode in categoryOfCargoNodes)
                {
                    if (categoryOfCargoNode != null && categoryOfCargoNode.HasChildNodes)
                    {
                        categoriesOfCargo.Add(categoryOfCargoNode.InnerText);
                    }
                }
                CategoryOfCargo = categoriesOfCargo.ToArray();
            }

            var categoryOfDangerousOrHazardousCargoNodes = node.SelectNodes("categoryOfDangerousOrHazardousCargo");
            if (categoryOfDangerousOrHazardousCargoNodes != null && categoryOfDangerousOrHazardousCargoNodes.Count > 0)
            {
                var categoriesOfCargo = new List<string>();
                foreach (XmlNode categoryOfCargoNode in categoryOfCargoNodes)
                {
                    if (categoryOfCargoNode != null && categoryOfCargoNode.HasChildNodes)
                    {
                        categoriesOfCargo.Add(categoryOfCargoNode.InnerText);
                    }
                }
                CategoryOfDangerousOrHazardousCargo = categoriesOfCargo.ToArray();
            }

            var categoryOfVesselNode = node.SelectSingleNode("categoryOfVessel", mgr);
            if (categoryOfVesselNode != null && categoryOfVesselNode.HasChildNodes)
            {
                CategoryOfVessel = categoryOfVesselNode.FirstChild.InnerText;
            }

            var categoryOfVesselRegistryNode = node.SelectSingleNode("categoryOfVesselRegistry", mgr);
            if (categoryOfVesselRegistryNode != null && categoryOfVesselRegistryNode.HasChildNodes)
            {
                CategoryOfVesselRegistry = categoryOfVesselRegistryNode.FirstChild.InnerText;
            }
            
            var logicalConnectivesNode = node.SelectSingleNode("logicalConnectives", mgr);
            if (logicalConnectivesNode != null && logicalConnectivesNode.HasChildNodes)
            {
                LogicalConnectives = logicalConnectivesNode.FirstChild.InnerText;
            }

            var thicknessOfIceCapabilityNode = node.SelectSingleNode("thicknessOfIceCapability", mgr);
            if (thicknessOfIceCapabilityNode != null && thicknessOfIceCapabilityNode.HasChildNodes)
            {
                ThicknessOfIceCapability = thicknessOfIceCapabilityNode.FirstChild.InnerText;
            }

            var vesselsMeasurementsNodes = node.SelectNodes("vesselsMeasurements", mgr);
            if (vesselsMeasurementsNodes != null && vesselsMeasurementsNodes.Count > 0)
            {
                var measurements = new List<VesselsMeasurement>();
                foreach(XmlNode vesselsMeasurementsNode in vesselsMeasurementsNodes)
                {
                    if (vesselsMeasurementsNode != null && vesselsMeasurementsNode.HasChildNodes)
                    {
                        var newMeasurement = new VesselsMeasurement();
                        newMeasurement.FromXml(vesselsMeasurementsNode, mgr);
                        measurements.Add(newMeasurement);
                    }
                }

                VesselsMeasurements = measurements.ToArray();
            }

            var vesselPerformanceNode = node.SelectSingleNode("vesselPerformance", mgr);
            if (vesselPerformanceNode != null && vesselPerformanceNode.HasChildNodes)
            {
                VesselPerformance = vesselPerformanceNode.FirstChild.InnerText;
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
