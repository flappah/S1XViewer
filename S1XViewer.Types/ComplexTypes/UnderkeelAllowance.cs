using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class UnderkeelAllowance : ComplexTypeBase, IUnderkeelAllowance
    {
        public string UnderkeelAllowanceFixed { get; set; } = string.Empty;
        public string UnderkeelAllowanceVariableBeamBased { get; set; } = string.Empty;
        public string UnderkeelAllowanceVariableDraughtBased { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new UnderkeelAllowance
            {
                UnderkeelAllowanceFixed = UnderkeelAllowanceFixed,
                UnderkeelAllowanceVariableBeamBased = UnderkeelAllowanceVariableBeamBased,
                UnderkeelAllowanceVariableDraughtBased = UnderkeelAllowanceVariableDraughtBased,
                Operation = Operation
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
            var underkeelAllowanceFixedNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}underkeelAllowanceFixed", mgr);
            if (underkeelAllowanceFixedNode != null && underkeelAllowanceFixedNode.HasChildNodes)
            {
                UnderkeelAllowanceFixed = underkeelAllowanceFixedNode.FirstChild?.InnerText ?? string.Empty;
            }

            var underkeelAllowanceVariableBeamBasedNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}underkeelAllowanceVariableBeamBased", mgr);
            if (underkeelAllowanceVariableBeamBasedNode != null && underkeelAllowanceVariableBeamBasedNode.HasChildNodes)
            {
                UnderkeelAllowanceVariableBeamBased = underkeelAllowanceVariableBeamBasedNode.FirstChild?.InnerText ?? string.Empty;
            }

            var underkeelAllowanceVariableDraughtBasedNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}underkeelAllowanceVariableDraughtBased", mgr);
            if (underkeelAllowanceVariableDraughtBasedNode != null && underkeelAllowanceVariableDraughtBasedNode.HasChildNodes)
            {
                UnderkeelAllowanceVariableDraughtBased = underkeelAllowanceVariableDraughtBasedNode.FirstChild?.InnerText ?? string.Empty;
            }

            var operationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}operation", mgr);
            if (operationNode != null && operationNode.HasChildNodes)
            {
                Operation = operationNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
