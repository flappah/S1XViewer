using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public abstract class CatalogueElement : GeoFeatureBase, ICatalogueElement
    {
        // new for v2.0
        public string AgencyResponsibleForProduction { get; set; } = string.Empty;
        public string[] CatalogueElementClassification { get; set; } = Array.Empty<string>();
        public string CatalogueElementIdentifier { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
        public string[] IMOMaritimeService { get; set; } = Array.Empty<string>();
        public string Keywords { get; set; } = string.Empty;
        public bool NotForNavigation { get; set; } = false;
        public IInformation[] Information { get; set; } = Array.Empty<Information>();
        public IOnlineResource OnlineResource { get; set; } = new OnlineResource();
        public ISupportFile[] SupportFile { get; set; } = Array.Empty<SupportFile>();
        public ITimeIntervalOfProduct TimeIntervalOfProduct = new TimeIntervalOfProduct();

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

            // coming from GeoFeatureBase
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

            var sourceIndicationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}sourceIndication", mgr);
            if (sourceIndicationNode != null && sourceIndicationNode.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndicationNode, mgr, nameSpacePrefix);
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

            //public string AgencyResponsibleForProduction { get; set; } = string.Empty;
            var agencyResponsibleForProductionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}agencyResponsibleForProduction", mgr);
            if (agencyResponsibleForProductionNode != null && agencyResponsibleForProductionNode.HasChildNodes)
            {
                AgencyResponsibleForProduction = agencyResponsibleForProductionNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string[] CatalogueElementClassification { get; set; } = new string[0];
            var catalogueElementClassificationNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}catalogueElementClassification", mgr);
            if (catalogueElementClassificationNodes != null && catalogueElementClassificationNodes.Count > 0)
            {
                var catalogueElements = new List<string>();
                foreach (XmlNode catalogueElementClassificationNode in catalogueElementClassificationNodes)
                {
                    if (catalogueElementClassificationNode != null && catalogueElementClassificationNode.HasChildNodes)
                    {
                        catalogueElements.Add(catalogueElementClassificationNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                CatalogueElementClassification = catalogueElements.ToArray();
            }

            //public string CatalogueElementIdentifier { get; set; } = string.Empty;
            var catalogueElementIdentifierNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}catalogueElementIdentifier", mgr);
            if (catalogueElementIdentifierNode == null)
            if (catalogueElementIdentifierNode != null && catalogueElementIdentifierNode.HasChildNodes)
            {
                CatalogueElementIdentifier = catalogueElementIdentifierNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string Classification { get; set; }
            var classificationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}classification", mgr);
            if (classificationNode != null && classificationNode.HasChildNodes)
            {
                Classification = classificationNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string[] IMOMaritimeService { get; set; } = new string[0];
            var imoMaritimeServiceNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}IMOMaritimeService", mgr);
            if (imoMaritimeServiceNodes != null && imoMaritimeServiceNodes.Count > 0)
            {
                var maritimeServiceNodes = new List<string>();
                foreach (XmlNode imoMaritimeServiceNode in imoMaritimeServiceNodes)
                {
                    if (imoMaritimeServiceNode != null && imoMaritimeServiceNode.HasChildNodes)
                    {
                        maritimeServiceNodes.Add(imoMaritimeServiceNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                IMOMaritimeService = maritimeServiceNodes.ToArray();
            }

            //public string Keywords { get; set; } = string.Empty;
            var keywordsNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}keywords", mgr);
            if (keywordsNode != null && keywordsNode.HasChildNodes)
            {
                Keywords = keywordsNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public bool NotForNavigation { get; set; } = false;
            var notForNavigationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}notForNavigation", mgr);
            if (notForNavigationNode != null && notForNavigationNode.HasChildNodes)
            {
                NotForNavigation = false; // default value is false
                if (bool.TryParse(notForNavigationNode.InnerText, out bool notForNavigationValue))
                {
                    NotForNavigation = notForNavigationValue;
                }
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

            //public OnlineResource OnlineResource { get; set; } = new OnlineResource();
            var onlineResourceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}onlineResource", mgr);
            if (onlineResourceNode != null && onlineResourceNode.HasChildNodes)
            {
                OnlineResource = new OnlineResource();
                OnlineResource.FromXml(onlineResourceNode, mgr, nameSpacePrefix);
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

            //public TimeIntervalOfProduct TimeIntervalOfProduct = new TimeIntervalOfProduct();
            var timeIntervalOfProductNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}timeIntervalOfProduct", mgr);
            if (timeIntervalOfProductNode != null && timeIntervalOfProductNode.HasChildNodes)
            {
                TimeIntervalOfProduct = new TimeIntervalOfProduct();
                TimeIntervalOfProduct.FromXml(timeIntervalOfProductNode, mgr, nameSpacePrefix);
            }

            return this;
        }
    }
}
