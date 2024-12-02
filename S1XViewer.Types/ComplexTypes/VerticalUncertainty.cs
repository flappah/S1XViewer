using S1XViewer.Types.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class VerticalUncertainty : ComplexTypeBase, IVerticalUncertainty
    {
        public double UncertaintyFixed { get; set; }
        public double UncertaintyVariable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new VerticalUncertainty
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
                if (double.TryParse(uncertaintyFixedNode.FirstChild?.InnerText,
                                    NumberStyles.Float,
                                    new CultureInfo("en-US"),
                                    out double uncertaintyFixed) == true)
                {
                    UncertaintyFixed = uncertaintyFixed;
                }
            }

            var uncertaintyVariableNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}uncertaintyVariable", mgr);
            if (uncertaintyVariableNode != null && uncertaintyVariableNode.HasChildNodes)
            {
                if (double.TryParse(uncertaintyVariableNode.FirstChild?.InnerText,
                                    NumberStyles.Float,
                                    new CultureInfo("en-US"),
                                    out double uncertaintyVariable) == true)
                {
                    UncertaintyVariable = uncertaintyVariable;
                }
            }

            return this;
        }
    }
}
