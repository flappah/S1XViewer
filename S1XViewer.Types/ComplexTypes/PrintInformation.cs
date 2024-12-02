using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class PrintInformation : ComplexTypeBase, IPrintInformation
    {
        public string PrintAgency { get; set; } = string.Empty;
        public string PrintNation { get; set; } = string.Empty;
        public string PrintSize { get; set; } = string.Empty;
        public string PrintWeek { get; set; } = string.Empty;
        public string PrintYear { get; set; } = string.Empty;
        public string ReprintEdition { get; set; } = string.Empty;
        public string ReprintNation { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new PrintInformation
            {
                PrintAgency = PrintAgency,
                PrintNation = PrintNation,
                PrintSize = PrintSize,
                PrintWeek = PrintWeek,
                PrintYear = PrintYear,
                ReprintEdition = ReprintEdition,
                ReprintNation = ReprintNation
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var printAgencyNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}printAgency", mgr);
            if (printAgencyNode != null && printAgencyNode.HasChildNodes)
            {
                PrintAgency = printAgencyNode.FirstChild?.InnerText ?? string.Empty;
            }

            var printNationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}printNation", mgr);
            if (printNationNode != null && printNationNode.HasChildNodes)
            {
                PrintNation = printNationNode.FirstChild?.InnerText ?? string.Empty;
            }

            var printSizeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}printSize", mgr);
            if (printSizeNode != null && printSizeNode.HasChildNodes)
            {
                PrintSize = printSizeNode.FirstChild?.InnerText ?? string.Empty;
            }

            var printWeekNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}printWeek", mgr);
            if (printWeekNode != null && printWeekNode.HasChildNodes)
            {
                PrintWeek = printWeekNode.FirstChild?.InnerText ?? string.Empty;
            }

            var printYearNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}printYear", mgr);
            if (printYearNode != null && printYearNode.HasChildNodes)
            {
                PrintYear = printYearNode.FirstChild?.InnerText ?? string.Empty;
            }

            var reprintEditionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}reprintEdition", mgr);
            if (reprintEditionNode != null && reprintEditionNode.HasChildNodes)
            {
                ReprintEdition = reprintEditionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var reprintNationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}reprintNation", mgr);
            if (reprintNationNode != null && reprintNationNode.HasChildNodes)
            {
                ReprintNation = reprintNationNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;

        }
    }
}
