using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public abstract class CatalogueElements : GeoFeatureBase, ICatalogueElements
    {
        public string Classification { get; set; }
        public string Copyright { get; set; }
        public string EditionDate { get; set; }
        public string EditionNumber { get; set; }
        public string HorizontalDatumReference { get; set; }
        public string IssueDate { get; set; }
        public string MarineResourceName { get; set; }
        public string MaximumDisplayScale { get; set; }
        public string MinimumDisplayScale { get; set; }
        public string ProductType { get; set; }
        public string Purpose { get; set; }
        public string SoundingDatum { get; set; }
        public string TimeIntervalOfProduct { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateNumber { get; set; }
        public string VerticalDatum { get; set; }
        // is defined in GeoFeatureBase public IFeatureName[] FeatureName { get; set; }
        public IGraphic[] Graphic { get; set; }
        public IInformation[] Information { get; set; }
        public IPayment[] Payment { get; set; }
        public IProducingAgency ProducingAgency { get; set; }
        // is defined in GeoFeatureBase public ISourceIndication SourceIndication { get; set; }
        public ISupportFile[] SupportFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null || !node.HasChildNodes) return this;

            var featureNameNodes = node.SelectNodes("featureName", mgr);
            if (featureNameNodes != null && featureNameNodes.Count > 0)
            {
                var featureNames = new List<FeatureName>();
                foreach (XmlNode featureNameNode in featureNameNodes)
                {
                    var newFeatureName = new FeatureName();
                    newFeatureName.FromXml(featureNameNode, mgr);
                    featureNames.Add(newFeatureName);
                }
                FeatureName = featureNames.ToArray();
            }

            var sourceIndication = node.SelectSingleNode("sourceIndication", mgr);
            if (sourceIndication != null && sourceIndication.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndication, mgr);
            }

            var textContentNodes = node.SelectNodes("textContent", mgr);
            if (textContentNodes != null && textContentNodes.Count > 0)
            {
                var textContents = new List<TextContent>();
                foreach (XmlNode textContentNode in textContentNodes)
                {
                    if (textContentNode != null && textContentNode.HasChildNodes)
                    {
                        var content = new TextContent();
                        content.FromXml(textContentNode, mgr);
                        textContents.Add(content);
                    }
                }
                TextContent = textContents.ToArray();
            }

            //public string Classification { get; set; }
            var classificationNode = node.SelectSingleNode("classification", mgr);
            if (classificationNode != null && classificationNode.HasChildNodes)
            {
                Classification = classificationNode.FirstChild.InnerText;
            }

            //public string Copyright { get; set; }
            var copyrightNode = node.SelectSingleNode("copyright", mgr);
            if (copyrightNode != null && copyrightNode.HasChildNodes)
            {
                Copyright = copyrightNode.FirstChild.InnerText;
            }

            //public string MaximumDisplayScale { get; set; }
            var maximumDisplayScaleNode = node.SelectSingleNode("maximumDisplayScale", mgr);
            if (maximumDisplayScaleNode != null && maximumDisplayScaleNode.HasChildNodes)
            {
                MaximumDisplayScale = maximumDisplayScaleNode.FirstChild.InnerText;
            }

            //public string MinimumDisplayScale { get; set; }
            var minimumDisplayScaleNode = node.SelectSingleNode("minimumDisplayScale", mgr);
            if (minimumDisplayScaleNode != null && minimumDisplayScaleNode.HasChildNodes)
            {
                MinimumDisplayScale = minimumDisplayScaleNode.FirstChild.InnerText;
            }

            //public string HorizontalDatumReference { get; set; }
            var horizontalDatumReferenceNode = node.SelectSingleNode("horizontalDatumReference", mgr);
            if (horizontalDatumReferenceNode != null && horizontalDatumReferenceNode.HasChildNodes)
            {
                HorizontalDatumReference = horizontalDatumReferenceNode.FirstChild.InnerText;
            }

            //public string VerticalDatum { get; set; }
            var verticalDatumNode = node.SelectSingleNode("verticalDatum", mgr);
            if (verticalDatumNode != null && verticalDatumNode.HasChildNodes)
            {
                VerticalDatum = verticalDatumNode.FirstChild.InnerText;
            }

            //public string SoundingDatum { get; set; }
            var soundingDatumNode = node.SelectSingleNode("soundingDatum", mgr);
            if (soundingDatumNode != null && soundingDatumNode.HasChildNodes)
            {
                SoundingDatum = soundingDatumNode.FirstChild.InnerText;
            }

            //public string ProductType { get; set; }
            var productTypeNode = node.SelectSingleNode("productType", mgr);
            if (productTypeNode != null && productTypeNode.HasChildNodes)
            {
                ProductType = productTypeNode.FirstChild.InnerText;
            }

            //public string IssueDate { get; set; }
            var issueDateNode = node.SelectSingleNode("issueDate", mgr);
            if (issueDateNode != null && issueDateNode.HasChildNodes)
            {
                IssueDate = issueDateNode.FirstChild.InnerText;
            }

            //public string Purpose { get; set; }
            var purposeNode = node.SelectSingleNode("purpose", mgr);
            if (purposeNode != null && purposeNode.HasChildNodes)
            {
                Purpose = purposeNode.FirstChild.InnerText;
            }

            //public string MarineResourceName { get; set; }
            var marineResourceNameNode = node.SelectSingleNode("marineResourceName", mgr);
            if (marineResourceNameNode != null && marineResourceNameNode.HasChildNodes)
            {
                MarineResourceName = marineResourceNameNode.FirstChild.InnerText;
            }

            //public string UpdateDate { get; set; }
            var updateDateNode = node.SelectSingleNode("updateDate", mgr);
            if (updateDateNode != null && updateDateNode.HasChildNodes)
            {
                UpdateDate = updateDateNode.FirstChild.InnerText;
            }

            //public string UpdateNumber { get; set; }
            var updateNumberNode = node.SelectSingleNode("updateNumber", mgr);
            if (updateNumberNode != null && updateNumberNode.HasChildNodes)
            {
                UpdateNumber = updateNumberNode.FirstChild.InnerText;
            }

            //public string EditionDate { get; set; }
            var editionDateNode = node.SelectSingleNode("editionDate", mgr);
            if (editionDateNode != null && editionDateNode.HasChildNodes)
            {
                EditionDate = editionDateNode.FirstChild.InnerText;
            }

            //public string EditionNumber { get; set; }
            var editionNumberNode = node.SelectSingleNode("editionNumber", mgr);
            if (editionNumberNode != null && editionNumberNode.HasChildNodes)
            {
                EditionNumber = editionNumberNode.FirstChild.InnerText;
            }

            //public string TimeIntervalOfProduct { get; set; }
            var timeIntervalOfProductNode = node.SelectSingleNode("timeIntervalOfProduct", mgr);
            if (timeIntervalOfProductNode != null && timeIntervalOfProductNode.HasChildNodes)
            {
                TimeIntervalOfProduct = timeIntervalOfProductNode.FirstChild.InnerText;
            }

            //public IInformation[] Information { get; set; }
            var informationNodes = node.SelectNodes("information", mgr);
            if (informationNodes != null && informationNodes.Count > 0)
            {
                var informations = new List<Information>();
                foreach (XmlNode informationNode in informationNodes)
                {
                    if (informationNode != null && informationNode.HasChildNodes)
                    {
                        var newInformation = new Information();
                        newInformation.FromXml(informationNode, mgr);
                        informations.Add(newInformation);
                    }
                }
                Information = informations.ToArray();
            }

            //public IPayment[] Payment { get; set; }
            var paymentNodes = node.SelectNodes("payment", mgr);
            if (paymentNodes != null && paymentNodes.Count > 0)
            {
                var payments = new List<Payment>();
                foreach (XmlNode paymentNode in paymentNodes)
                {
                    if (paymentNode != null && paymentNode.HasChildNodes)
                    {
                        var newPayment = new Payment();
                        newPayment.FromXml(paymentNode, mgr);
                        payments.Add(newPayment);
                    }
                }
                Payment = payments.ToArray();
            }

            //public IProducingAgency ProducingAgency { get; set; }
            var producingAgencyNode = node.SelectSingleNode("producingAgency", mgr);
            if (producingAgencyNode != null && producingAgencyNode.HasChildNodes)
            {
                ProducingAgency = new ProducingAgency();
                ProducingAgency.FromXml(producingAgencyNode, mgr);
            }

            //public IGraphic[] Graphic { get; set; }
            var graphicNodes = node.SelectNodes("graphic", mgr);
            if (graphicNodes != null && graphicNodes.Count > 0)
            {
                var graphics = new List<Graphic>();
                foreach (XmlNode graphicNode in graphicNodes)
                {
                    if (graphicNode != null && graphicNode.HasChildNodes)
                    {
                        var newGraphic = new Graphic();
                        newGraphic.FromXml(graphicNode, mgr);
                        graphics.Add(newGraphic);
                    }
                }
                Graphic = graphics.ToArray();
            }

            //public ISupportFile[] SupportFile { get; set; }
            var supportFileNodes = node.SelectNodes("supportFile", mgr);
            if (supportFileNodes != null && supportFileNodes.Count > 0)
            {
                var supportFiles = new List<SupportFile>();
                foreach (XmlNode supportFileNode in supportFileNodes)
                {
                    if (supportFileNode != null && supportFileNode.HasChildNodes)
                    {
                        var newSupportFile = new SupportFile();
                        newSupportFile.FromXml(supportFileNode, mgr);
                        supportFiles.Add(newSupportFile);
                    }
                }
                SupportFile = supportFiles.ToArray();
            }

            return this;
        }
    }
}
