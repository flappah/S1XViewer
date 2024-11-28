using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class S100Service : CatalogueElement, IS100Service, IS128Feature
    {
        public bool CompressionFlag { get; set; }
        public string EncodingFormat { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceStatus { get; set; } = string.Empty;
        public string TypeOfProductFormat { get; set; } = string.Empty;
        public IProductSpecification ProductSpecification { get; set; } = new ProductSpecification();
        public IServiceSpecification ServiceSpecification { get; set; } = new ServiceSpecification();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new S100Service()
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

                CompressionFlag = CompressionFlag,
                EncodingFormat = EncodingFormat,
                ServiceName = ServiceName,
                ServiceStatus = ServiceStatus,
                TypeOfProductFormat = TypeOfProductFormat,
                ProductSpecification = ProductSpecification == null ? new ProductSpecification() : ProductSpecification.DeepClone() as IProductSpecification,
                ServiceSpecification = ServiceSpecification == null ? new ServiceSpecification() : ServiceSpecification.DeepClone() as IServiceSpecification
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

            base.FromXml(node, mgr); // run the CatalogueElements xml interpreter

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

            //public string ServiceName { get; set; } = string.Empty;
            var serviceNameNode = node.SelectSingleNode("serviceName", mgr);
            if (serviceNameNode == null)
            {
                serviceNameNode = node.SelectSingleNode("S128:serviceName", mgr);
            }
            if (serviceNameNode != null && serviceNameNode.HasChildNodes)
            {
                ServiceName = serviceNameNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string ServiceStatus { get; set; } = string.Empty;
            var serviceStatusNode = node.SelectSingleNode("serviceStatus", mgr);
            if (serviceStatusNode == null)
            {
                serviceStatusNode = node.SelectSingleNode("S128:serviceStatus", mgr);
            }
            if (serviceStatusNode != null && serviceStatusNode.HasChildNodes)
            {
                ServiceStatus = serviceStatusNode.FirstChild?.InnerText ?? string.Empty;
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

            //public IServiceSpecification ServiceSpecification { get; set; } = new ServiceSpecification();
            var serviceSpecificationNode = node.SelectSingleNode("serviceSpecification", mgr);
            if (serviceSpecificationNode == null)
            {
                serviceSpecificationNode = node.SelectSingleNode("S128:serviceSpecification", mgr);
            }
            if (serviceSpecificationNode != null && serviceSpecificationNode.HasChildNodes)
            {
                ServiceSpecification = new ServiceSpecification();
                ServiceSpecification.FromXml(serviceSpecificationNode, mgr);
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
