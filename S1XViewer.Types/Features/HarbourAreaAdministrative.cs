using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class HarbourAreaAdministrative : Layout, IS131Feature, IHarbourAreaAdministrative
    {
        public string UNLocationCode { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string ISPSLevel { get; set; } = string.Empty;
        public string ApplicableLoadLineZone { get; set; } = string.Empty;
        public string[] CategoryOfHarbourFacility { get; set; } = Array.Empty<string>();
        public IGeneralHarbourInformation GeneralHarbourInformation { get; set; } = new GeneralHarbourInformation();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IFeature DeepClone()
        {
            return new HarbourAreaAdministrative()
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
                Links = Links == null
                    ? Array.Empty<ILink>()
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink),
                UNLocationCode = UNLocationCode,
                Nationality = Nationality,
                ISPSLevel = ISPSLevel,
                ApplicableLoadLineZone = ApplicableLoadLineZone,
                CategoryOfHarbourFacility = CategoryOfHarbourFacility == null ? new string[0] : Array.ConvertAll(CategoryOfHarbourFacility, chf => chf),
                GeneralHarbourInformation = GeneralHarbourInformation == null ? new GeneralHarbourInformation() : GeneralHarbourInformation.DeepClone() as IGeneralHarbourInformation
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

            var uNLocationCodeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}uNLocationCode", mgr);
            if (uNLocationCodeNode != null && uNLocationCodeNode.HasChildNodes)
            {
                UNLocationCode = uNLocationCodeNode.FirstChild?.InnerText ?? string.Empty;
            }

            var nationalityNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}nationality", mgr);
            if (nationalityNode != null && nationalityNode.HasChildNodes)
            {
                Nationality = nationalityNode.FirstChild?.InnerText ?? string.Empty;
            }

            var applicableLoadLineZoneNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}applicableLoadLineZone", mgr);
            if (applicableLoadLineZoneNode != null && applicableLoadLineZoneNode.HasChildNodes)
            {
                ApplicableLoadLineZone = applicableLoadLineZoneNode.FirstChild?.InnerText ?? string.Empty;
            }

            var iSPSLevelNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}iSPSLevel", mgr);
            if (iSPSLevelNode != null && iSPSLevelNode.HasChildNodes)
            {
                ISPSLevel = iSPSLevelNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfHarbourFacilityNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfHarbourFacility", mgr);
            if (categoryOfHarbourFacilityNodes != null && categoryOfHarbourFacilityNodes.Count > 0)
            {
                var harbourFacilities = new List<string>();
                foreach (XmlNode categoryOfHarbourFacilityNode in categoryOfHarbourFacilityNodes)
                {
                    if (categoryOfHarbourFacilityNode != null && categoryOfHarbourFacilityNode.HasChildNodes && String.IsNullOrEmpty(categoryOfHarbourFacilityNode.FirstChild?.InnerText) == false)
                    {
                        harbourFacilities.Add(categoryOfHarbourFacilityNode.FirstChild?.InnerText);
                    }
                }

                harbourFacilities.Sort();
                CategoryOfHarbourFacility = harbourFacilities.ToArray();
            }

            var generalHarbourInformationnode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}generalHarbourInformation", mgr);
            if (generalHarbourInformationnode != null && generalHarbourInformationnode.HasChildNodes)
            {
                GeneralHarbourInformation = new GeneralHarbourInformation();
                GeneralHarbourInformation.FromXml(generalHarbourInformationnode, mgr, nameSpacePrefix);
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            return "HRBARE91";
        }
    }
}
