using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class FeatureObjectIdentifier : ComplexTypeBase, IFeatureObjectIdentifier
    {
        public string Agency { get; set; }
        public string FeatureIdentificationNumber { get; set; }
        public string FeatureIdentificationSubdivision { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node != null && node.HasChildNodes)
            {
                var agencyNode = node.SelectSingleNode("s100:agency", mgr);
                if (agencyNode != null)
                {
                    Agency = agencyNode.FirstChild.InnerText;
                }

                var featureIdentificationNumberNode = node.SelectSingleNode("s100:featureIdentificationNumber", mgr);
                if (featureIdentificationNumberNode != null)
                {
                    FeatureIdentificationNumber = featureIdentificationNumberNode.FirstChild.InnerText;
                }

                var featureIdentificationSubdivisionNode = node.SelectSingleNode("s100:featureIdentificationSubdivision", mgr);
                if (featureIdentificationSubdivisionNode != null)
                {
                    FeatureIdentificationSubdivision = featureIdentificationSubdivisionNode.FirstChild.InnerText;
                }
            }

            return this;
        }
    }
}
