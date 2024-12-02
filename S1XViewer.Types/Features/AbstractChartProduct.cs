using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public abstract class AbstractChartProduct : CatalogueElements, IAbstractChartProduct
    {
        public string ChartNumber { get; set; } = string.Empty;
        public string DistributionStatus { get; set; } = string.Empty;
        public string[] CompilationScale { get; set; } = Array.Empty<string>();
        public string SpecificUsage { get; set; } = string.Empty;
        public string ProducerCode { get; set; } = string.Empty;
        public string OriginalChartNumber { get; set; } = string.Empty;
        public string ProducerNation { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null || !node.HasChildNodes) return this;

            base.FromXml(node, mgr, nameSpacePrefix);

            //public string ChartNumber { get; set; }
            var chartNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}chartNumber", mgr);
            if (chartNumberNode != null && chartNumberNode.HasChildNodes)
            {
                ChartNumber = chartNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string DistributionStatus { get; set; }
            var distributionStatusNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}distributionStatus", mgr);
            if (distributionStatusNode != null && distributionStatusNode.HasChildNodes)
            {
                DistributionStatus = distributionStatusNode?.InnerText ?? string.Empty;
            }

            //public string CompilationScale { get; set; }
            var compilationScaleNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}compilationScale", mgr);
            if (compilationScaleNodes != null && compilationScaleNodes.Count > 0)
            {
                var compilationScales = new List<string>();
                foreach (XmlNode categoryOfCargoNode in compilationScaleNodes)
                {
                    if (categoryOfCargoNode != null && categoryOfCargoNode.HasChildNodes)
                    {
                        compilationScales.Add(categoryOfCargoNode.InnerText);
                    }
                }
                CompilationScale = compilationScales.ToArray();
            }

            //public string SpecificUsage { get; set; }
            var specificUsageNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}specificUsage", mgr);
            if (specificUsageNode != null && specificUsageNode.HasChildNodes)
            {
                SpecificUsage = specificUsageNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string ProducerCode { get; set; }
            var producerCodeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}producerCode", mgr);
            if (producerCodeNode != null && producerCodeNode.HasChildNodes)
            {
                ProducerCode = producerCodeNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string OriginalChartNumber { get; set; }
            var originalChartNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}originalChartNumber", mgr);
            if (originalChartNumberNode != null && originalChartNumberNode.HasChildNodes)
            {
                OriginalChartNumber = originalChartNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string ProducerNation { get; set; }
            var producerNationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}producerNation", mgr);
            if (producerNationNode != null && producerNationNode.HasChildNodes)
            {
                ProducerNation = producerNationNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
