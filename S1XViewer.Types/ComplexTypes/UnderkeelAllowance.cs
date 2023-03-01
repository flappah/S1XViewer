using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class UnderkeelAllowance : ComplexTypeBase, IUnderkeelAllowance
    {
        public string UnderkeelAllowanceFixed { get; set; }
        public string UnderkeelAllowanceVariableBeamBased { get; set; }
        public string UnderkeelAllowanceVariableDraughtBased { get; set; }
        public string Operation { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var underkeelAllowanceFixedNode = node.SelectSingleNode("underkeelAllowanceFixed", mgr);
            if (underkeelAllowanceFixedNode != null && underkeelAllowanceFixedNode.HasChildNodes)
            {
                UnderkeelAllowanceFixed = underkeelAllowanceFixedNode.FirstChild.InnerText;
            }

            var underkeelAllowanceVariableBeamBasedNode = node.SelectSingleNode("underkeelAllowanceVariableBeamBased", mgr);
            if (underkeelAllowanceVariableBeamBasedNode != null && underkeelAllowanceVariableBeamBasedNode.HasChildNodes)
            {
                UnderkeelAllowanceVariableBeamBased = underkeelAllowanceVariableBeamBasedNode.FirstChild.InnerText;
            }

            var underkeelAllowanceVariableDraughtBasedNode = node.SelectSingleNode("underkeelAllowanceVariableDraughtBased", mgr);
            if (underkeelAllowanceVariableDraughtBasedNode != null && underkeelAllowanceVariableDraughtBasedNode.HasChildNodes)
            {
                UnderkeelAllowanceVariableDraughtBased = underkeelAllowanceVariableDraughtBasedNode.FirstChild.InnerText;
            }

            var operationNode = node.SelectSingleNode("operation", mgr);
            if (operationNode != null && operationNode.HasChildNodes)
            {
                Operation = operationNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
