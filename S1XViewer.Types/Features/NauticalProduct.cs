using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class NauticalProduct : CatalogueElements, INauticalProduct, IS128Feature
    {
        public IProductSpecification ProductSpecification { get; set; }
        public IOnlineResource OnlineResource { get; set; }
        // TextContent is defined in GeoFeatureBase as an array. NauticalProducts uses only element 0!
        public IServiceSpecification ServiceSpecification { get; set; }
        public string PublicationNumber { get; set; }
        public string DataSetName { get; set; }
        public string Version { get; set; }
        public string ServiceStatus { get; set; }
        public string Keyword { get; set; }
        public string ServiceDesign { get; set; }
        public string ISBN { get; set; }
        public string TypeOfProductFormat { get; set; }

        /// <summary>
        ///     Provides a deepcloned version of the object
        /// </summary>
        /// <returns>IS1XxFeatureObject</returns>
        public override IFeature DeepClone()
        {
            return new NauticalProduct
            {
                Id = Id,
                Geometry = Geometry,
                Classification = Classification,
                Copyright = Copyright,
                MaximumDisplayScale = MaximumDisplayScale,
                MinimumDisplayScale = MinimumDisplayScale,
                HorizontalDatumReference = HorizontalDatumReference,
                VerticalDatum = VerticalDatum,
                SoundingDatum = SoundingDatum,
                ProductType = ProductType,
                IssueDate = IssueDate,
                Purpose = Purpose,
                MarineResourceName = MarineResourceName,
                UpdateDate = UpdateDate,
                UpdateNumber = UpdateNumber,
                EditionDate = EditionDate,
                EditionNumber = EditionNumber,
                FeatureName = FeatureName == null
                    ? new IFeatureName[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                Information = Information == null
                    ? new IInformation[0]
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                SourceIndication = SourceIndication == null
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                Payment = Payment == null
                    ? new IPayment[0]
                    : Array.ConvertAll(Payment, p => p.DeepClone() as IPayment),
                ProducingAgency = ProducingAgency == null
                    ? new ProducingAgency()
                    : ProducingAgency.DeepClone() as IProducingAgency,
                SupportFile = SupportFile == null
                    ? new ISupportFile[0]
                    : Array.ConvertAll(SupportFile, s => s.DeepClone() as ISupportFile),
                ProductSpecification = ProductSpecification == null
                    ? new ProductSpecification()
                    : ProductSpecification.DeepClone() as IProductSpecification,
                TextContent = TextContent == null || TextContent.Length == 0
                    ? new TextContent[0]
                    : new[] { TextContent[0] },
                OnlineResource = OnlineResource == null
                    ? new OnlineResource()
                    : OnlineResource.DeepClone() as IOnlineResource,
                ServiceSpecification = ServiceSpecification == null
                    ? new ServiceSpecification()
                    : ServiceSpecification.DeepClone() as IServiceSpecification,
                PublicationNumber = PublicationNumber,
                DataSetName = DataSetName,
                Version = Version,
                ServiceStatus = ServiceStatus,
                Keyword = Keyword,
                ServiceDesign = ServiceDesign,
                ISBN = ISBN,
                TypeOfProductFormat = TypeOfProductFormat,
                Links = Links == null
                    ? new ILink[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
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

            if (node.HasChildNodes)
            {
                if (node.Attributes?.Count > 0 && node.Attributes.Contains("gml:id") == true)
                {
                    Id = node.Attributes["gml:id"].InnerText;
                }
            }

            base.FromXml(node, mgr);

            //public IProductSpecification ProductSpecification { get; set; }
            var productSpecificationNode = node.SelectSingleNode("productSpecification", mgr);
            if (productSpecificationNode != null && productSpecificationNode.HasChildNodes)
            {
                ProductSpecification = new ProductSpecification();
                ProductSpecification.FromXml(productSpecificationNode, mgr);
            }

            //public IOnlineResource OnlineResource { get; set; }
            var onlineResourceNode = node.SelectSingleNode("onlineResource", mgr);
            if (onlineResourceNode != null && onlineResourceNode.HasChildNodes)
            {
                OnlineResource = new OnlineResource();
                OnlineResource.FromXml(onlineResourceNode, mgr);
            }

            //public string ServiceSpecification { get; set; }
            var serviceSpecificationNode = node.SelectSingleNode("serviceSpecification", mgr);
            if (serviceSpecificationNode != null && serviceSpecificationNode.HasChildNodes)
            {
                ServiceSpecification = new ServiceSpecification();
                ServiceSpecification.FromXml(serviceSpecificationNode, mgr);
            }

            //public string PublicationNumber { get; set; }
            var publicationNumberNode = node.SelectSingleNode("publicationNumber", mgr);
            if (publicationNumberNode != null && publicationNumberNode.HasChildNodes)
            {
                PublicationNumber = publicationNumberNode.InnerText;
            }

            //public string DataSetName { get; set; }
            var dataSetNameNode = node.SelectSingleNode("dataSetName", mgr);
            if (dataSetNameNode != null && dataSetNameNode.HasChildNodes)
            {
                DataSetName = dataSetNameNode.InnerText;
            }

            //public string Version { get; set; }
            var versionNode = node.SelectSingleNode("version", mgr);
            if (versionNode != null && versionNode.HasChildNodes)
            {
                Version = versionNode.InnerText;
            }

            //public string ServiceStatus { get; set; }
            var serviceStatusNode = node.SelectSingleNode("serviceStatus", mgr);
            if (serviceStatusNode != null && serviceStatusNode.HasChildNodes)
            {
                ServiceStatus = serviceStatusNode.InnerText;
            }

            //public string Keyword { get; set; }
            var keywordsNode = node.SelectSingleNode("keyword", mgr);
            if (keywordsNode != null && keywordsNode.HasChildNodes)
            {
                Keyword = keywordsNode.InnerText;
            }

            //public string ServiceDesign { get; set; }
            var serviceDesignNode = node.SelectSingleNode("serviceDesign", mgr);
            if (serviceDesignNode != null && serviceDesignNode.HasChildNodes)
            {
                ServiceDesign = serviceDesignNode.InnerText;
            }

            //public string ISBN { get; set; }
            var isbnNode = node.SelectSingleNode("ISBN", mgr);
            if (isbnNode != null && isbnNode.HasChildNodes)
            {
                ISBN = isbnNode.InnerText;
            }

            //public string TypeOfProductFormat { get; set; }
            var typeOfProductFormatNode = node.SelectSingleNode("typeOfProductFormat", mgr);
            if (typeOfProductFormatNode != null && typeOfProductFormatNode.HasChildNodes)
            {
                TypeOfProductFormat = typeOfProductFormatNode.InnerText;
            }

            var linkNodes = node.SelectNodes("*[boolean(@xlink:href)]", mgr);
            if (linkNodes != null && linkNodes.Count > 0)
            {
                var links = new List<Link>();
                foreach (XmlNode linkNode in linkNodes)
                {
                    var newLink = new Link();
                    newLink.FromXml(linkNode, mgr);
                    links.Add(newLink);
                }
                Links = links.ToArray();
            }

            return this;
        }
    }
}
