using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class HarbourAreaSection : Layout, IS131Feature, IHarbourAreaSection
    {
        public string CategoryOfPortSection { get; set; } = string.Empty;
        public string[] CategoryOfHarbourFacility { get; set; } = Array.Empty<string>();
        public string iSPSLevel { get; set; } = string.Empty;
        public string FacilitiesLayoutDescription { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new HarbourAreaSection()
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
                CategoryOfPortSection = CategoryOfPortSection,
                CategoryOfHarbourFacility = CategoryOfHarbourFacility == null ? new string[0] : CategoryOfHarbourFacility,
                iSPSLevel = iSPSLevel,
                FacilitiesLayoutDescription = FacilitiesLayoutDescription
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

            var categoryOfPortSectionNode = node.SelectSingleNode("categoryOfPortSection", mgr);
            if (categoryOfPortSectionNode != null && categoryOfPortSectionNode.HasChildNodes)
            {
                CategoryOfPortSection = categoryOfPortSectionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfHarbourFacilityNodes = node.SelectNodes("categoryOfHarbourFacility", mgr);
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

            var iSPSLevelNode = node.SelectSingleNode("iSPSLevel", mgr);
            if (iSPSLevelNode != null && iSPSLevelNode.HasChildNodes)
            {
                iSPSLevel = iSPSLevelNode.FirstChild?.InnerText ?? string.Empty;
            }

            var facilitiesLayoutDescriptionNode = node.SelectSingleNode("facilitiesLayoutDescription", mgr);
            if (facilitiesLayoutDescriptionNode != null && facilitiesLayoutDescriptionNode.HasChildNodes)
            {
                FacilitiesLayoutDescription = facilitiesLayoutDescriptionNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            if (String.IsNullOrEmpty(CategoryOfPortSection) == false)
            {
                switch (CategoryOfPortSection.Trim())
                {
                    case "1":
                        return "HRBSEC96";

                    case "3":
                        return "HRBSEC97";

                    case "8":
                        return "HRBSEC92";

                    case "9":
                        return "HRBSEC93";

                    case "11":
                        return "HRBSEC94";

                    case "12":
                        return "HRBSEC95";

                    default:
                        return "QUESMRK1";
                }
            }
            else if (CategoryOfHarbourFacility.Length > 0) 
            { 
                if (CategoryOfHarbourFacility.ToList().Where(q => q.Trim().Equals("1")).Count() > 0) 
                {
                    return "ROLROL01";
                }
                else if (CategoryOfHarbourFacility.ToList().Where(q => q.Trim().Equals("4")).Count() > 0)
                {
                    return "HRBFAC09";
                }
                else if (CategoryOfHarbourFacility.ToList().Where(q => q.Trim().Equals("5")).Count() > 0)
                {
                    return "SMCFAC02";
                }
                else if (CategoryOfHarbourFacility.ToList().Where(q => q.Trim().Equals("6")).Count() > 0)
                {
                    return "HRBFAC91";
                }
                else if (CategoryOfHarbourFacility.ToList().Where(q => q.Trim().Equals("9")).Count() > 0)
                {
                    return "HRBFAC93";
                }
                else if (CategoryOfHarbourFacility.ToList().Where(q => q.Trim().Equals("14")).Count() > 0)
                {
                    return "HRBFAC95";
                }
                else if (CategoryOfHarbourFacility.ToList().Where(q => q.Trim().Equals("15")).Count() > 0)
                {
                    return "HRBFAC96";
                }
                else if (CategoryOfHarbourFacility.ToList().Where(q => q.Trim().Equals("16")).Count() > 0)
                {
                    return "HRBFAC97";
                }
                else if (CategoryOfHarbourFacility.ToList().Where(q => q.Trim().Equals("17")).Count() > 0)
                {
                    return "HRBFAC98";
                }
                else
                {
                    return "CHINFO07";
                }
            }

            return "CHINFO07";
        }
    }
}
