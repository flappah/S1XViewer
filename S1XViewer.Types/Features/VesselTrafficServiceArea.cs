﻿using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class VesselTrafficServiceArea : GeoFeatureBase, IVesselTrafficServiceArea, IS122Feature, IS127Feature
    {
        // data
        public string CategoryOfVesselTrafficService { get; set; } = string.Empty;
        public string ServiceAccessProcedure { get; set; } = string.Empty;
        public string RequirementsForMaintenanceOfListeningWatch { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new VesselTrafficServiceArea
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
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                TextContent = TextContent == null
                    ? Array.Empty<TextContent>()
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                CategoryOfVesselTrafficService = CategoryOfVesselTrafficService,
                ServiceAccessProcedure = ServiceAccessProcedure,
                RequirementsForMaintenanceOfListeningWatch = RequirementsForMaintenanceOfListeningWatch,
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
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr, nameSpacePrefix);

            var categoryOfVesselTrafficServiceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfVesselTrafficService", mgr);
            if (categoryOfVesselTrafficServiceNode != null && categoryOfVesselTrafficServiceNode.HasChildNodes)
            {
                CategoryOfVesselTrafficService = categoryOfVesselTrafficServiceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var serviceAccessProcedureNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}serviceAccessProcedure", mgr);
            if (serviceAccessProcedureNode != null)
            {
                ServiceAccessProcedure = serviceAccessProcedureNode.FirstChild?.InnerText ?? string.Empty;
            }

            var requirementsForMaintenanceOfListeningWatchNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}requirementsForMaintenanceOfListeningWatch", mgr);
            if (requirementsForMaintenanceOfListeningWatchNode != null && requirementsForMaintenanceOfListeningWatchNode.HasChildNodes)
            {
                RequirementsForMaintenanceOfListeningWatch = requirementsForMaintenanceOfListeningWatchNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            return "";
        }
    }
}
