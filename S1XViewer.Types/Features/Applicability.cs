using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Applicability : InformationFeatureBase, IApplicability, IS122Feature, IS123Feature, IS127Feature, IFeature
    {
        public string Ballast { get; set; } = string.Empty;
        public string[] CategoryOfCargo { get; set; } = Array.Empty<string>();
        public string[] CategoryOfDangerousOrHazardousCargo { get; set; } = Array.Empty<string>();
        public string CategoryOfVessel { get; set; } = string.Empty;
        public string CategoryOfVesselRegistry { get; set; } = string.Empty;
        public string LogicalConnectives { get; set; } = string.Empty;
        public string ThicknessOfIceCapability { get; set; } = string.Empty;
        public IVesselsMeasurement[] VesselsMeasurements { get; set; } = Array.Empty<IVesselsMeasurement>();
        public IInformation[] Information { get; set; } = Array.Empty<Information>();
        public string VesselPerformance { get; set; } = string.Empty;

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
                    ? Array.Empty<DateRange>()
                    : Array.ConvertAll(PeriodicDateRange, p => p.DeepClone() as IDateRange),
                SourceIndication = SourceIndication == null
                    ? Array.Empty<SourceIndication>()
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
                    ? Array.Empty<Information>()
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                LogicalConnectives = LogicalConnectives,
                ThicknessOfIceCapability = ThicknessOfIceCapability,
                VesselsMeasurements = VesselsMeasurements == null
                    ? Array.Empty<VesselsMeasurement>()
                    : Array.ConvertAll(VesselsMeasurements, v => v.DeepClone() as IVesselsMeasurement),
                VesselPerformance = VesselPerformance,
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

            base.FromXml(node, mgr);
            
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

            return this;
        }
    }
}
