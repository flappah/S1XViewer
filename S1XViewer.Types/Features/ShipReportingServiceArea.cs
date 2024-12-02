using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Features
{
    public class ShipReportingServiceArea : ReportableServiceArea, IShipReportingServiceArea, IS127Feature
    {
        public string ServiceAccessProcedure { get; set; } = string.Empty;
        public string RequirementsForMaintenanceOfListeningWatch { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new ShipReportingServiceArea
            {
                FeatureName = FeatureName == null
                    ? new IFeatureName[] { new FeatureName() }
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
                    ? Array.Empty<ITextContent>()
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                ServiceAccessProcedure = ServiceAccessProcedure,
                RequirementsForMaintenanceOfListeningWatch = RequirementsForMaintenanceOfListeningWatch,
                Links = Links == null
                    ? Array.Empty<ILink>()
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

            var serviceAccessProcedureNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}serviceaccessprocedure", mgr);
            if (serviceAccessProcedureNode != null && serviceAccessProcedureNode.HasChildNodes)
            {
                ServiceAccessProcedure = serviceAccessProcedureNode.FirstChild?.InnerText ?? string.Empty;
            }

            var requirementsForMaintenanceOfListeningWatchNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}requirementsformaintenanceoflisteningwatch", mgr);
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
