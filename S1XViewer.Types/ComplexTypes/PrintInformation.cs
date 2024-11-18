using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class PrintInformation : ComplexTypeBase, IPrintInformation
    {
        public string PrintAgency { get; set; }
        public string PrintNation { get; set; }
        public string PrintSize { get; set; }
        public string PrintWeek { get; set; }
        public string PrintYear { get; set; }
        public string ReprintEdition { get; set; }
        public string ReprintNation { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var printAgencyNode = node.SelectSingleNode("printAgency");
            if (printAgencyNode != null && printAgencyNode.HasChildNodes)
            {
                PrintAgency = printAgencyNode.FirstChild.InnerText;
            }

            var printNationNode = node.SelectSingleNode("printNation");
            if (printNationNode != null && printNationNode.HasChildNodes)
            {
                PrintNation = printNationNode.FirstChild.InnerText;
            }

            var printSizeNode = node.SelectSingleNode("printSize");
            if (printSizeNode != null && printSizeNode.HasChildNodes)
            {
                PrintSize = printSizeNode.FirstChild.InnerText;
            }

            var printWeekNode = node.SelectSingleNode("printWeek");
            if (printWeekNode != null && printWeekNode.HasChildNodes)
            {
                PrintWeek = printWeekNode.FirstChild.InnerText;
            }

            var printYearNode = node.SelectSingleNode("printYear");
            if (printYearNode != null && printYearNode.HasChildNodes)
            {
                PrintYear = printYearNode.FirstChild.InnerText;
            }

            var reprintEditionNode = node.SelectSingleNode("reprintEdition");
            if (reprintEditionNode != null && reprintEditionNode.HasChildNodes)
            {
                ReprintEdition = reprintEditionNode.FirstChild.InnerText;
            }

            var reprintNationNode = node.SelectSingleNode("reprintNation");
            if (reprintNationNode != null && reprintNationNode.HasChildNodes)
            {
                ReprintNation = reprintNationNode.FirstChild.InnerText;
            }

            return this;

        }
    }
}
