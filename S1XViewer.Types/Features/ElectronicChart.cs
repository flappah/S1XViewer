using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class ElectronicChart : AbstractChartProduct, IElectronicChart, IS128Feature
    {
        public IProductSpecification ProductSpecification { get; set; }
        public string[] DatasetName { get; set; }
        public string TnpUpdate { get; set; }
        public string TypeOfProductFormat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new ElectronicChart
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
                ChartNumber = ChartNumber,
                DistributionStatus = DistributionStatus,
                CompilationScale = CompilationScale,
                SpecificUsage = SpecificUsage,
                ProducerCode = ProducerCode,
                OriginalChartNumber = OriginalChartNumber,
                ProducerNation = ProducerNation,
                ProductSpecification = ProductSpecification == null
                    ? new ProductSpecification()
                    : ProductSpecification.DeepClone() as IProductSpecification,
                DatasetName = DatasetName == null
                    ? new string[0]
                    : Array.ConvertAll(DatasetName, s => s),
                TnpUpdate = TnpUpdate,
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
            if (node == null || !node.HasChildNodes) return this;

            if (node.FirstChild.Attributes != null && node.FirstChild.Attributes.Count > 0)
            {
                Id = node.FirstChild.Attributes["gml:id"].InnerText;
            }

            base.FromXml(node, mgr);

            //public IProductSpecification ProductSpecification { get; set; }
            var productSpecificationNode = node.FirstChild.SelectSingleNode("productSpecification", mgr);
            if (productSpecificationNode != null && productSpecificationNode.HasChildNodes)
            {
                ProductSpecification = new ProductSpecification();
                ProductSpecification.FromXml(productSpecificationNode, mgr);
            }

            //public string[] DatasetName { get; set; }
            var datasetNameNodes = node.FirstChild.SelectNodes("datasetName", mgr);
            if (datasetNameNodes != null && datasetNameNodes.Count > 0)
            {
                var dataSetNames = new List<string>();
                foreach (XmlNode datasetNameNode in datasetNameNodes)
                {
                    if (datasetNameNode != null && datasetNameNode.HasChildNodes)
                    {
                        var dataSetName = datasetNameNode.FirstChild.InnerText;
                        dataSetNames.Add(dataSetName);
                    }
                }

                DatasetName = dataSetNames.ToArray();
            }

            //public string TnpUpdate { get; set; }
            var tnpUpdateNode = node.FirstChild.SelectSingleNode("tnpUpdate", mgr);
            if (tnpUpdateNode != null && tnpUpdateNode.HasChildNodes)
            {
                TnpUpdate = tnpUpdateNode.FirstChild.InnerText;
            }

            //public string TypeOfProductFormat { get; set; }
            var typeOfProductFormatNode = node.FirstChild.SelectSingleNode("typeOfProductFormat", mgr);
            if (typeOfProductFormatNode != null && typeOfProductFormatNode.HasChildNodes)
            {
                TypeOfProductFormat = typeOfProductFormatNode.FirstChild.InnerText;
            }

            var linkNodes = node.FirstChild.SelectNodes("*[boolean(@xlink:href)]", mgr);
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
