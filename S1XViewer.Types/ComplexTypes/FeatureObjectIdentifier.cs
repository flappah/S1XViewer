using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class FeatureObjectIdentifier : ComplexTypeBase, IFeatureObjectIdentifier
    {
        public string Agency { get; set; } = string.Empty;
        public string FeatureIdentificationNumber { get; set; } = string.Empty;
        public string FeatureIdentificationSubdivision { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new FeatureObjectIdentifier
            {
                Agency = Agency,
                FeatureIdentificationNumber = FeatureIdentificationNumber,
                FeatureIdentificationSubdivision = FeatureIdentificationSubdivision
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node != null && node.HasChildNodes)
            {
                var agencyNode = node.SelectSingleNode("S100:agency", mgr);
                if (agencyNode != null)
                {
                    Agency = agencyNode.FirstChild?.InnerText ?? string.Empty;
                }

                var featureIdentificationNumberNode = node.SelectSingleNode("S100:featureIdentificationNumber", mgr);
                if (featureIdentificationNumberNode != null)
                {
                    FeatureIdentificationNumber = featureIdentificationNumberNode.FirstChild?.InnerText ?? string.Empty;
                }

                var featureIdentificationSubdivisionNode = node.SelectSingleNode("S100:featureIdentificationSubdivision", mgr);
                if (featureIdentificationSubdivisionNode != null)
                {
                    FeatureIdentificationSubdivision = featureIdentificationSubdivisionNode.FirstChild?.InnerText ?? string.Empty;
                }
            }

            return this;
        }
    }
}
