using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public abstract class NavigationalProduct : CatalogueElement, INavigationalProduct
    {
        public int ApproximateGridResolution { get; set; }
        public int[] CompilationScale { get; set; } = new int[0];
        public string DistributionStatus { get; set; } = string.Empty;
        public string[] NavigationPurpose { get; set; } = new string[0];
        public int OptimumDisplayScale { get; set; }
        public string OriginalProductNumber { get; set; } = string.Empty;
        public string ProducerNation { get; set; } = string.Empty;
        public string ProductNumber { get; set; } = string.Empty;
        public string SpecificUsage { get; set; } = string.Empty;
        public string UpdateDate { get; set; } = string.Empty;
        public string UpdateNumber { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null || !node.HasChildNodes) return this;

            base.FromXml(node, mgr); // run the CatalogueElements Xml interpreter

            //public int ApproximateGridResolution { get; set; }
            var approximateGridResolutionNode = node.SelectSingleNode("approximateGridResolution", mgr);
            if (approximateGridResolutionNode == null)
            {
                approximateGridResolutionNode = node.SelectSingleNode("S128:approximateGridResolution", mgr);
            }
            if (approximateGridResolutionNode != null && approximateGridResolutionNode.HasChildNodes)
            {
                if (int.TryParse(approximateGridResolutionNode.FirstChild?.InnerText, out int approximateGridResolutionValue))
                {
                    ApproximateGridResolution = approximateGridResolutionValue;
                }
            }

            //public int[] CompilationScale { get; set; } = new int[0];
            var compilationScaleNodes = node.SelectNodes("compilationScale");
            if (compilationScaleNodes == null || compilationScaleNodes.Count == 0)
            {
                compilationScaleNodes = node.SelectNodes("S128:compilationScale", mgr);
            }
            if (compilationScaleNodes != null && compilationScaleNodes.Count > 0)
            {
                var compilationScales = new List<int>();
                foreach (XmlNode compilationScaleNode in compilationScaleNodes)
                {
                    if (compilationScaleNode != null && compilationScaleNode.HasChildNodes)
                    {
                        if (int.TryParse(compilationScaleNode.FirstChild?.InnerText, out int compilationScalesValue))
                        {
                            compilationScales.Add(compilationScalesValue);
                        }
                    }
                }

                CompilationScale = compilationScales.ToArray();
            }

            //public string DistributionStatus { get; set; } = string.Empty;
            var distributionStatusNode = node.SelectSingleNode("distributionStatus", mgr);
            if (distributionStatusNode == null)
            {
                distributionStatusNode = node.SelectSingleNode("S128:distributionStatus", mgr);
            }
            if (distributionStatusNode != null && distributionStatusNode.HasChildNodes)
            {
                DistributionStatus = distributionStatusNode.FirstChild?.InnerText ?? string.Empty;
            }

            //editionNumber -> base parser
            //maximumDisplayScale -> base parser
            //minimumDisplayScale -> base parser

            //public string[] NavigationPurpose { get; set; } = new string[0];
            var navigationPurposeNodes = node.SelectNodes("navigationPurpose");
            if (navigationPurposeNodes == null || navigationPurposeNodes.Count == 0)
            {
                navigationPurposeNodes = node.SelectNodes("S128:navigationPurpose", mgr);
            }
            if (navigationPurposeNodes != null && navigationPurposeNodes.Count > 0)
            {
                var navigationPurposes = new List<string>();
                foreach (XmlNode navigationPurposeNode in navigationPurposeNodes)
                {
                    if (navigationPurposeNode != null && navigationPurposeNode.HasChildNodes)
                    {
                        navigationPurposes.Add(navigationPurposeNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                NavigationPurpose = navigationPurposes.ToArray();
            }

            //public int OptimumDisplayScale { get; set; }
            var optimumDisplayScaleNode = node.SelectSingleNode("optimumDisplayScale", mgr);
            if (optimumDisplayScaleNode == null)
            {
                optimumDisplayScaleNode = node.SelectSingleNode("S128:optimumDisplayScale", mgr);
            }
            if (optimumDisplayScaleNode != null && optimumDisplayScaleNode.HasChildNodes)
            {
                if (int.TryParse(optimumDisplayScaleNode.FirstChild?.InnerText, out int optimumDisplayScaleValue))
                {
                    OptimumDisplayScale = optimumDisplayScaleValue;
                }
            }

            //public string OriginalProductNumber { get; set; } = string.Empty;
            var originalProductNumberValue = node.SelectSingleNode("originalProductNumber", mgr);
            if (originalProductNumberValue == null)
            {
                originalProductNumberValue = node.SelectSingleNode("S128:originalProductNumber", mgr);
            }
            if (originalProductNumberValue != null && originalProductNumberValue.HasChildNodes)
            {
                OriginalProductNumber = originalProductNumberValue.FirstChild?.InnerText ?? string.Empty;
            }

            //public string ProducerNation { get; set; } = string.Empty;
            var producerNationNode = node.SelectSingleNode("producerNation", mgr);
            if (producerNationNode == null)
            {
                producerNationNode = node.SelectSingleNode("S128:producerNation", mgr);
            }
            if (producerNationNode != null && producerNationNode.HasChildNodes)
            {
                ProducerNation = producerNationNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string ProductNumber { get; set; } = string.Empty;
            var productNumberNode = node.SelectSingleNode("productNumber", mgr);
            if (productNumberNode == null)
            {
                productNumberNode = node.SelectSingleNode("S128:productNumber", mgr);
            }
            if (productNumberNode != null && productNumberNode.HasChildNodes)
            {
                ProductNumber = productNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string SpecificUsage { get; set; } = string.Empty;
            var specificUsageNode = node.SelectSingleNode("specificUsage", mgr);
            if (productNumberNode == null)
            {
                productNumberNode = node.SelectSingleNode("S128:specificUsage", mgr);
            }
            if (productNumberNode != null && productNumberNode.HasChildNodes)
            {
                SpecificUsage = productNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string UpdateDate { get; set; }
            var updateDateNode = node.SelectSingleNode("updateDate", mgr);
            if (updateDateNode != null && updateDateNode.HasChildNodes)
            {
                UpdateDate = updateDateNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string UpdateNumber { get; set; }
            var updateNumberNode = node.SelectSingleNode("updateNumber", mgr);
            if (updateNumberNode != null && updateNumberNode.HasChildNodes)
            {
                UpdateNumber = updateNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
