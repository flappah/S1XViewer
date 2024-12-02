using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class VesselsMeasurement : ComplexTypeBase, IVesselsMeasurement
    {
        public string ComparisonOperator { get; set; } = string.Empty;
        public string VesselsCharacteristics { get; set; } = string.Empty;
        public string VesselsCharacteristicsValue { get; set; } = string.Empty;
        public string VesselsCharacteristicsUnit { get; set; } = string.Empty;

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var comparisonOperatorNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}comparisonOperator", mgr);
            if (comparisonOperatorNode != null && comparisonOperatorNode.HasChildNodes)
            {
                ComparisonOperator = comparisonOperatorNode.FirstChild?.InnerText ?? string.Empty;
            }

            var vesselsCharacteristicsNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}vesselsCharacteristics", mgr);
            if (vesselsCharacteristicsNode != null && vesselsCharacteristicsNode.HasChildNodes)
            {
                VesselsCharacteristics = vesselsCharacteristicsNode.FirstChild?.InnerText ?? string.Empty;
            }

            var vesselsCharacteristicsValueNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}vesselsCharacteristicsValue", mgr);
            if (vesselsCharacteristicsValueNode != null && vesselsCharacteristicsValueNode.HasChildNodes)
            {
                VesselsCharacteristicsValue = vesselsCharacteristicsValueNode.FirstChild?.InnerText ?? string.Empty;
            }

            var vesselsCharacteristicsUnitNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}vesselsCharacteristicsUnit", mgr);
            if (vesselsCharacteristicsUnitNode != null && vesselsCharacteristicsUnitNode.HasChildNodes)
            {
                VesselsCharacteristicsUnit = vesselsCharacteristicsUnitNode.FirstChild?.InnerText ?? string.Empty;
            }
            
            return this;
        }
    }
}
