using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class PhysicalProduct : NavigationalProduct, IPhysicalProduct, IS128Feature
    {
        public string ISBN { get; set; } = string.Empty;
        public string PublicationNumber { get; set; } = string.Empty;
        public DateTime ReferenceToNM { get; set; }
        public string TypeOfPaper { get; set; } = string.Empty;
        public IPrintInformation PrintInformation { get; set; } = new PrintInformation();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new PhysicalProduct()
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

                AgencyResponsibleForProduction = AgencyResponsibleForProduction,
                CatalogueElementClassification = CatalogueElementClassification == null ? new string[0] : Array.ConvertAll(CatalogueElementClassification, s => s),
                CatalogueElementIdentifier = CatalogueElementIdentifier,
                IMOMaritimeService = IMOMaritimeService == null ? new string[0] : Array.ConvertAll(IMOMaritimeService, s => s),
                Keywords = Keywords,
                NotForNavigation = NotForNavigation,
                OnlineResource = OnlineResource == null ? new OnlineResource() : OnlineResource.DeepClone() as IOnlineResource,
                TimeIntervalOfProduct = TimeIntervalOfProduct == null ? new TimeIntervalOfProduct() : TimeIntervalOfProduct.DeepClone() as ITimeIntervalOfProduct,

                Classification = Classification,
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

                ISBN = ISBN,
                PublicationNumber = PublicationNumber,
                ReferenceToNM = ReferenceToNM,
                TypeOfPaper = TypeOfPaper
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
            if (node == null || !node.HasChildNodes) return this;

            base.FromXml(node, mgr, nameSpacePrefix); // run the NavigationalProduct xml interpreter

            //public string ISBN { get; set; } = string.Empty;
            var isbnNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}isbn", mgr);
            if (isbnNode != null && isbnNode.HasChildNodes)
            {
                ISBN = isbnNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string PublicationNumber { get; set; } = string.Empty;
            var publicationNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}publicationNumber", mgr);
            if (publicationNumberNode != null && publicationNumberNode.HasChildNodes)
            {
                PublicationNumber = publicationNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string TypeOfPaper { get; set; } = string.Empty;
            var typeOfPaperNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}typeOfPaper", mgr);
            if (typeOfPaperNode != null && typeOfPaperNode.HasChildNodes)
            {
                TypeOfPaper = typeOfPaperNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public IPrintInformation PrintInformation { get; set; } = new PrintInformation();
            var printInformationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}printInformation", mgr);
            if (printInformationNode != null && printInformationNode.HasChildNodes)
            {
                PrintInformation = new PrintInformation();
                PrintInformation.FromXml(printInformationNode, mgr, nameSpacePrefix);
            }

            //public DateTime ReferenceToNM { get; set; }
            var referenceToNMNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}referenceToNM", mgr);
            if (referenceToNMNode != null && referenceToNMNode.HasChildNodes)
            {
                if (DateTime.TryParse(referenceToNMNode.FirstChild?.InnerText, out DateTime referenceToNMValue))
                {
                    ReferenceToNM = referenceToNMValue;
                }
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
