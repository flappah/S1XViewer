using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class HorizontalPositionalUncertainty : ComplexTypeBase, IHorizontalPositionalUncertainty
    {
        public string UncertaintyFixed { get; set; } = string.Empty;   
        public string UncertaintyVariable { get; set; } = string.Empty;

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var uncertaintyFixedNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}uncertaintyFixed", mgr);
            if (uncertaintyFixedNode != null && uncertaintyFixedNode.HasChildNodes)
            {
                UncertaintyFixed = uncertaintyFixedNode.FirstChild?.InnerText ?? string.Empty;
            }

            var uncertaintyVariableNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}uncertaintyVariable", mgr);
            if (uncertaintyVariableNode != null && uncertaintyVariableNode.HasChildNodes)
            {
                UncertaintyVariable = uncertaintyVariableNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
