using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class HorizontalPositionalUncertainty : ComplexTypeBase, IHorizontalPositionalUncertainty
    {
        public string UncertaintyFixed { get; set; }
        public string UncertaintyVariable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new HorizontalPositionalUncertainty
            {
                UncertaintyFixed = UncertaintyFixed,
                UncertaintyVariable = UncertaintyVariable
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
            var uncertaintyFixedNode = node.SelectSingleNode("uncertaintyFixed", mgr);
            if (uncertaintyFixedNode != null && uncertaintyFixedNode.HasChildNodes)
            {
                UncertaintyFixed = uncertaintyFixedNode.FirstChild.InnerText;
            }

            var uncertaintyVariableNode = node.SelectSingleNode("uncertaintyVariable", mgr);
            if (uncertaintyVariableNode != null && uncertaintyVariableNode.HasChildNodes)
            {
                UncertaintyVariable = uncertaintyVariableNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
