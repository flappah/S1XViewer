using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class PaperChart : AbstractChartProduct, IPaperChart, IS128Feature
    {
        public string FrameDimensions { get; set; }
        public bool MainPanel { get; set; }
        public string TypeOfPaper { get; set; }
        public IPrintInformation PrintInformation { get; set; }
        public IReferenceToNM ReferenceToNM { get; set; }
        public string ISBN { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new PaperChart
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
                FrameDimensions = FrameDimensions,
                MainPanel = MainPanel,
                TypeOfPaper = TypeOfPaper,
                PrintInformation = PrintInformation == null
                    ? new PrintInformation()
                    : PrintInformation.DeepClone() as IPrintInformation,
                ReferenceToNM = ReferenceToNM == null
                    ? new ReferenceToNM()
                    : ReferenceToNM.DeepClone() as IReferenceToNM,
                ISBN = ISBN,
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

            //public string FrameDimensions { get; set; }
            var frameDimensionsNode = node.FirstChild.SelectSingleNode("frameDimensions", mgr);
            if (frameDimensionsNode != null && frameDimensionsNode.HasChildNodes)
            {
                FrameDimensions = frameDimensionsNode.FirstChild.InnerText;
            }

            //public bool MainPanel { get; set; }
            var mainPanelNode = node.FirstChild.SelectSingleNode("mainPanel", mgr);
            if (mainPanelNode != null && mainPanelNode.HasChildNodes)
            {
                MainPanel = false; // default value is false
                if (bool.TryParse(mainPanelNode.FirstChild.InnerText, out bool mainPanelValue))
                {
                    MainPanel = mainPanelValue;
                }
            }

            //public string TypeOfPaper { get; set; }
            var typeOfPaperNode = node.FirstChild.SelectSingleNode("typeOfPaper", mgr);
            if (typeOfPaperNode != null && typeOfPaperNode.HasChildNodes)
            {
                TypeOfPaper = typeOfPaperNode.FirstChild.InnerText;
            }

            //public IPrintInformation PrintInformation { get; set; }
            var printInformationNode = node.FirstChild.SelectSingleNode("printInformation", mgr);
            if (printInformationNode != null && printInformationNode.HasChildNodes)
            {
                PrintInformation = new PrintInformation();
                PrintInformation.FromXml(printInformationNode, mgr);
            }

            //public IReferenceToNM ReferenceToNM { get; set; }
            var referenceToNMNode = node.FirstChild.SelectSingleNode("referenceToNM", mgr);
            if (referenceToNMNode != null && referenceToNMNode.HasChildNodes)
            {
                ReferenceToNM = new ReferenceToNM();
                ReferenceToNM.FromXml(referenceToNMNode, mgr);
            }

            //public string ISBN { get; set; }
            var isbnNode = node.FirstChild.SelectSingleNode("ISBN", mgr);
            if (isbnNode != null && isbnNode.HasChildNodes)
            {
                ISBN = isbnNode.FirstChild.InnerText;
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
