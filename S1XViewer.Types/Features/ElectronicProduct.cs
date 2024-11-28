using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class ElectronicProduct : NavigationalProduct, IElectronicProduct, IS128Feature
    {
        public bool CompressionFlag { get; set; }
        public string DatasetName { get; set; } = string.Empty;
        public string EncodingFormat { get; set; } = string.Empty;
        public DateTime IssueDateTime { get; set; }
        public string TypeOfProductFormat { get; set; } = string.Empty;
        public IProductSpecification ProductSpecification { get; set; } = new ProductSpecification();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new ElectronicProduct()
            {
                Id = Id,
                Geometry = Geometry,
                FeatureName = FeatureName == null
                    ? new IFeatureName[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null ? new DateRange() : FixedDateRange.DeepClone() as IDateRange,
                PeriodicDateRange = PeriodicDateRange == null ? new DateRange[0] : Array.ConvertAll(PeriodicDateRange, pdr => pdr as IDateRange),
                SourceIndication = SourceIndication == null
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                TextContent = TextContent == null ? new TextContent[0] : Array.ConvertAll(TextContent, tc => tc.DeepClone() as ITextContent),
                               
                TimeIntervalOfProduct = TimeIntervalOfProduct == null ? new TimeIntervalOfProduct() : TimeIntervalOfProduct.DeepClone() as ITimeIntervalOfProduct,

                Classification = Classification,
                
                Information = Information == null
                    ? new IInformation[0]
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                SupportFile = SupportFile == null
                    ? new ISupportFile[0]
                    : Array.ConvertAll(SupportFile, s => s.DeepClone() as ISupportFile),

                ApproximateGridResolution = ApproximateGridResolution,
                CompilationScale = CompilationScale == null ? new int[0] : Array.ConvertAll(CompilationScale, cs => cs),
                DistributionStatus = DistributionStatus,
                NavigationPurpose = NavigationPurpose == null ? new string[0] : Array.ConvertAll(NavigationPurpose, s => s),
                OptimumDisplayScale = OptimumDisplayScale,
                OriginalProductNumber = OriginalProductNumber,
                ProducerNation = ProducerNation,
                ProductNumber = ProductNumber,
                SpecificUsage = SpecificUsage,
                UpdateDate = UpdateDate,
                UpdateNumber = UpdateNumber,

                CompressionFlag = CompressionFlag,
                DatasetName = DatasetName,
                EncodingFormat = EncodingFormat,
                IssueDateTime = IssueDateTime,
                TypeOfProductFormat = TypeOfProductFormat,
                ProductSpecification = ProductSpecification == null ? new ProductSpecification() : ProductSpecification.DeepClone() as IProductSpecification,
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
            if (node == null || !node.HasChildNodes) return this;

            base.FromXml(node, mgr); // run the NavigationalProduct xml interpreter

            //public bool CompressionFlag { get; set; }
            var compressionFlagNode = node.SelectSingleNode("compressionFlag", mgr);
            if (compressionFlagNode == null)
            {
                compressionFlagNode = node.SelectSingleNode("S128:compressionFlag", mgr);
            }
            if (compressionFlagNode != null && compressionFlagNode.HasChildNodes)
            {
                if (bool.TryParse(compressionFlagNode.FirstChild?.InnerText, out bool compressionFlagValue))
                {
                    CompressionFlag = compressionFlagValue;
                }
            }

            //public string DatasetName { get; set; } = string.Empty;
            var datasetNameNode = node.SelectSingleNode("datasetName", mgr);
            if (datasetNameNode == null)
            {
                datasetNameNode = node.SelectSingleNode("S128:datasetName", mgr);
            }
            if (datasetNameNode != null && datasetNameNode.HasChildNodes)
            {
                DatasetName = datasetNameNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string EncodingFormat { get; set; } = string.Empty;
            var encodingFormatNode = node.SelectSingleNode("encodingFormat", mgr);
            if (encodingFormatNode == null)
            {
                encodingFormatNode = node.SelectSingleNode("S128:encodingFormat", mgr);
            }
            if (encodingFormatNode != null && encodingFormatNode.HasChildNodes)
            {
                EncodingFormat = encodingFormatNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public DateTime IssueDateTime { get; set; }
            var issueDateNode = node.SelectSingleNode("issueDate", mgr);
            var issueTimeNode = node.SelectSingleNode("issueTime", mgr);
            if (issueDateNode == null)
            {
                issueDateNode = node.SelectSingleNode("S128:issueDate", mgr);
            }
            if (issueTimeNode == null)
            {
                issueTimeNode = node.SelectSingleNode("S128:issueTime", mgr);
            }
            if (issueDateNode != null && issueDateNode.HasChildNodes)
            {
                var issueDate = issueDateNode.FirstChild?.InnerText ?? string.Empty;

                if (issueTimeNode != null && issueTimeNode.HasChildNodes)
                {
                    var issueTime = issueTimeNode.FirstChild?.InnerText ?? string.Empty;

                    if (DateTime.TryParse($"{issueDate}T{issueTime}", out DateTime dateTimeValue))
                    {
                        IssueDateTime = dateTimeValue;
                    }
                }
            }

            //public string TypeOfProductFormat { get; set; } = string.Empty;
            var typeOfProductFormatNode = node.SelectSingleNode("typeOfProductFormat", mgr);
            if (typeOfProductFormatNode == null)
            {
                typeOfProductFormatNode = node.SelectSingleNode("S128:typeOfProductFormat", mgr);
            }
            if (typeOfProductFormatNode != null && typeOfProductFormatNode.HasChildNodes)
            {
                TypeOfProductFormat = typeOfProductFormatNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public IProductSpecification ProductSpecification { get; set; } = new ProductSpecification();
            var productSpecificationNode = node.SelectSingleNode("productSpecification", mgr);
            if (productSpecificationNode == null)
            {
                productSpecificationNode = node.SelectSingleNode("S128:productSpecification", mgr);
            }
            if (productSpecificationNode != null && productSpecificationNode.HasChildNodes)
            {
                ProductSpecification = new ProductSpecification();
                ProductSpecification.FromXml(productSpecificationNode, mgr);
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
