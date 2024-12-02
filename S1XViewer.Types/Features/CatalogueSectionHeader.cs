using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Features
{
    public class CatalogueSectionHeader : InformationFeatureBase, IS128Feature, ICatalogueSectionHeader
    {
        public int CatalogueSectionNumber { get; set; }
        public string CatalogueSectionTitle { get; set; } = string.Empty;
        public IInformation Information { get; set; } = new Information();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new CatalogueSectionHeader
            {
                CatalogueSectionNumber = CatalogueSectionNumber,
                CatalogueSectionTitle = CatalogueSectionTitle,
                Information = Information == null
                    ? new Information()
                    : Information.DeepClone() as IInformation
            };
        }

        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null || !node.HasChildNodes) return this;

            //public int CatalogueSectionNumber { get; set; }
            var catalogueSectionNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}catalogueSectionNumber", mgr);
            if (catalogueSectionNumberNode != null && catalogueSectionNumberNode.HasChildNodes)
            {
                if (int.TryParse(catalogueSectionNumberNode.FirstChild?.InnerText, out int catalogueSectionNumberValue))
                {
                    CatalogueSectionNumber = catalogueSectionNumberValue;
                }
            }

            //public string CatalogueSectionTitle { get; set; }
            var catalogueSectionTitleNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}catalogueSectionTitle", mgr);
            if (catalogueSectionTitleNode != null && catalogueSectionTitleNode.HasChildNodes)
            {
                CatalogueSectionTitle = catalogueSectionTitleNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public IInformation Information { get; set; } = new Information();
            var informationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}information", mgr);
            if (informationNode != null && informationNode.HasChildNodes)
            {
                Information = new Information();
                Information.FromXml(informationNode, mgr, nameSpacePrefix);
            }

            return this;
        }
    }
}
