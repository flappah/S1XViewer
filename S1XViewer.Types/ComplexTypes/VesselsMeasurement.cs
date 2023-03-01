using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class VesselsMeasurement : ComplexTypeBase, IVesselsMeasurement
    {
        public string ComparisonOperator { get; set; }
        public string VesselsCharacteristics { get; set; }
        public string VesselsCharacteristicsValue { get; set; }
        public string VesselsCharacteristicsUnit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new VesselsMeasurement
            {
                ComparisonOperator = ComparisonOperator,
                VesselsCharacteristics = VesselsCharacteristics,
                VesselsCharacteristicsUnit = VesselsCharacteristicsUnit,
                VesselsCharacteristicsValue = VesselsCharacteristicsValue
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var comparisonOperatorNode = node.SelectSingleNode("comparisonOperator", mgr);
            if (comparisonOperatorNode != null && comparisonOperatorNode.HasChildNodes)
            {
                ComparisonOperator = comparisonOperatorNode.FirstChild.InnerText;
            }

            var vesselsCharacteristicsNode = node.SelectSingleNode("vesselsCharacteristics", mgr);
            if (vesselsCharacteristicsNode != null && vesselsCharacteristicsNode.HasChildNodes)
            {
                VesselsCharacteristics = vesselsCharacteristicsNode.FirstChild.InnerText;
            }

            var vesselsCharacteristicsValueNode = node.SelectSingleNode("vesselsCharacteristicsValue", mgr);
            if (vesselsCharacteristicsValueNode != null && vesselsCharacteristicsValueNode.HasChildNodes)
            {
                VesselsCharacteristicsValue = vesselsCharacteristicsValueNode.FirstChild.InnerText;
            }

            var vesselsCharacteristicsUnitNode = node.SelectSingleNode("vesselsCharacteristicsUnit", mgr);
            if (vesselsCharacteristicsUnitNode != null && vesselsCharacteristicsUnitNode.HasChildNodes)
            {
                VesselsCharacteristicsUnit = vesselsCharacteristicsUnitNode.FirstChild.InnerText;
            }
            
            return this;
        }
    }
}
