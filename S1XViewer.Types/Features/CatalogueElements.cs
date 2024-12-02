using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public abstract class CatalogueElements : GeoFeatureBase, ICatalogueElements
    {
        public string Classification { get; set; } = string.Empty;
        public string Copyright { get; set; } = string.Empty;
        public string EditionDate { get; set; } = string.Empty;
        public string EditionNumber { get; set; } = string.Empty;
        public string HorizontalDatumReference { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;
        public string MarineResourceName { get; set; } = string.Empty;
        public string MaximumDisplayScale { get; set; } = string.Empty;
        public string MinimumDisplayScale { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string SoundingDatum { get; set; } = string.Empty;
        public string TimeIntervalOfProduct { get; set; } = string.Empty;
        public string UpdateDate { get; set; } = string.Empty;
        public string UpdateNumber { get; set; } = string.Empty;
        public string VerticalDatum { get; set; } = string.Empty;
        // is defined in GeoFeatureBase public IFeatureName[] FeatureName { get; set; }
        // is defined in GeoFeatureBase public IGraphic[] Graphic { get; set; } = Array.Empty<Graphic>();
        public IInformation[] Information { get; set; } = Array.Empty<Information>();
        public IPayment[] Payment { get; set; } = Array.Empty<Payment>();
        public IProducingAgency ProducingAgency { get; set; } = new ProducingAgency();
        // is defined in GeoFeatureBase public ISourceIndication SourceIndication { get; set; }
        public ISupportFile[] SupportFile { get; set; } = Array.Empty<SupportFile>();   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null || !node.HasChildNodes) return this;

            var featureNameNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}featureName", mgr);
            if (featureNameNodes != null && featureNameNodes.Count > 0)
            {
                var featureNames = new List<FeatureName>();
                foreach (XmlNode featureNameNode in featureNameNodes)
                {
                    var newFeatureName = new FeatureName();
                    newFeatureName.FromXml(featureNameNode, mgr, nameSpacePrefix);
                    featureNames.Add(newFeatureName);
                }
                FeatureName = featureNames.ToArray();
            }

            var sourceIndication = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}sourceIndication", mgr);
            if (sourceIndication != null && sourceIndication.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndication, mgr, nameSpacePrefix);
            }

            var textContentNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}textContent", mgr);
            if (textContentNodes != null && textContentNodes.Count > 0)
            {
                var textContents = new List<TextContent>();
                foreach (XmlNode textContentNode in textContentNodes)
                {
                    if (textContentNode != null && textContentNode.HasChildNodes)
                    {
                        var content = new TextContent();
                        content.FromXml(textContentNode, mgr, nameSpacePrefix);
                        textContents.Add(content);
                    }
                }
                TextContent = textContents.ToArray();
            }

            //public string Classification { get; set; }
            var classificationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}classification", mgr);
            if (classificationNode != null && classificationNode.HasChildNodes)
            {
                Classification = classificationNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string Copyright { get; set; }
            var copyrightNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}copyright", mgr);
            if (copyrightNode != null && copyrightNode.HasChildNodes)
            {
                Copyright = copyrightNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string MaximumDisplayScale { get; set; }
            var maximumDisplayScaleNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}maximumDisplayScale", mgr);
            if (maximumDisplayScaleNode != null && maximumDisplayScaleNode.HasChildNodes)
            {
                MaximumDisplayScale = maximumDisplayScaleNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string MinimumDisplayScale { get; set; }
            var minimumDisplayScaleNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}minimumDisplayScale", mgr);
            if (minimumDisplayScaleNode != null && minimumDisplayScaleNode.HasChildNodes)
            {
                MinimumDisplayScale = minimumDisplayScaleNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string HorizontalDatumReference { get; set; }
            var horizontalDatumReferenceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}horizontalDatumReference", mgr);
            if (horizontalDatumReferenceNode != null && horizontalDatumReferenceNode.HasChildNodes)
            {
                HorizontalDatumReference = horizontalDatumReferenceNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string VerticalDatum { get; set; }
            var verticalDatumNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}verticalDatum", mgr);
            if (verticalDatumNode != null && verticalDatumNode.HasChildNodes)
            {
                VerticalDatum = verticalDatumNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string SoundingDatum { get; set; }
            var soundingDatumNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}soundingDatum", mgr);
            if (soundingDatumNode != null && soundingDatumNode.HasChildNodes)
            {
                SoundingDatum = soundingDatumNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string ProductType { get; set; }
            var productTypeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}productType", mgr);
            if (productTypeNode != null && productTypeNode.HasChildNodes)
            {
                ProductType = productTypeNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string IssueDate { get; set; }
            var issueDateNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}issueDate", mgr);
            if (issueDateNode != null && issueDateNode.HasChildNodes)
            {
                IssueDate = issueDateNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string Purpose { get; set; }
            var purposeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}purpose", mgr);
            if (purposeNode != null && purposeNode.HasChildNodes)
            {
                Purpose = purposeNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string MarineResourceName { get; set; }
            var marineResourceNameNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}marineResourceName", mgr);
            if (marineResourceNameNode != null && marineResourceNameNode.HasChildNodes)
            {
                MarineResourceName = marineResourceNameNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string UpdateDate { get; set; }
            var updateDateNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}updateDate", mgr);
            if (updateDateNode != null && updateDateNode.HasChildNodes)
            {
                UpdateDate = updateDateNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string UpdateNumber { get; set; }
            var updateNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}updateNumber", mgr);
            if (updateNumberNode != null && updateNumberNode.HasChildNodes)
            {
                UpdateNumber = updateNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string EditionDate { get; set; }
            var editionDateNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}editionDate", mgr);
            if (editionDateNode != null && editionDateNode.HasChildNodes)
            {
                EditionDate = editionDateNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string EditionNumber { get; set; }
            var editionNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}editionNumber", mgr);
            if (editionNumberNode != null && editionNumberNode.HasChildNodes)
            {
                EditionNumber = editionNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string TimeIntervalOfProduct { get; set; }
            var timeIntervalOfProductNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}timeIntervalOfProduct", mgr);
            if (timeIntervalOfProductNode != null && timeIntervalOfProductNode.HasChildNodes)
            {
                TimeIntervalOfProduct = timeIntervalOfProductNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public IInformation[] Information { get; set; }
            var informationNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}information", mgr);
            if (informationNodes != null && informationNodes.Count > 0)
            {
                var informations = new List<Information>();
                foreach (XmlNode informationNode in informationNodes)
                {
                    if (informationNode != null && informationNode.HasChildNodes)
                    {
                        var newInformation = new Information();
                        newInformation.FromXml(informationNode, mgr, nameSpacePrefix);
                        informations.Add(newInformation);
                    }
                }
                Information = informations.ToArray();
            }

            //public IPayment[] Payment { get; set; }
            var paymentNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}payment", mgr);
            if (paymentNodes != null && paymentNodes.Count > 0)
            {
                var payments = new List<Payment>();
                foreach (XmlNode paymentNode in paymentNodes)
                {
                    if (paymentNode != null && paymentNode.HasChildNodes)
                    {
                        var newPayment = new Payment();
                        newPayment.FromXml(paymentNode, mgr, nameSpacePrefix);
                        payments.Add(newPayment);
                    }
                }
                Payment = payments.ToArray();
            }

            //public IProducingAgency ProducingAgency { get; set; }
            var producingAgencyNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}producingAgency", mgr);
            if (producingAgencyNode != null && producingAgencyNode.HasChildNodes)
            {
                ProducingAgency = new ProducingAgency();
                ProducingAgency.FromXml(producingAgencyNode, mgr, nameSpacePrefix);
            }

            //public IGraphic[] Graphic { get; set; }
            var graphicNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}graphic", mgr);
            if (graphicNodes != null && graphicNodes.Count > 0)
            {
                var graphics = new List<Graphic>();
                foreach (XmlNode graphicNode in graphicNodes)
                {
                    if (graphicNode != null && graphicNode.HasChildNodes)
                    {
                        var newGraphic = new Graphic();
                        newGraphic.FromXml(graphicNode, mgr, nameSpacePrefix);
                        graphics.Add(newGraphic);
                    }
                }
                Graphic = graphics.ToArray();
            }

            //public ISupportFile[] SupportFile { get; set; }
            var supportFileNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}supportFile", mgr);
            if (supportFileNodes != null && supportFileNodes.Count > 0)
            {
                var supportFiles = new List<SupportFile>();
                foreach (XmlNode supportFileNode in supportFileNodes)
                {
                    if (supportFileNode != null && supportFileNode.HasChildNodes)
                    {
                        var newSupportFile = new SupportFile();
                        newSupportFile.FromXml(supportFileNode, mgr, nameSpacePrefix);
                        supportFiles.Add(newSupportFile);
                    }
                }
                SupportFile = supportFiles.ToArray();
            }

            return this;
        }
    }
}
