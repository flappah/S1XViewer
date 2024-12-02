using S1XViewer.Types.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Orientation : ComplexTypeBase, IOrientation
    {
        public double OrientationUncertainty { get; set; } = 0.0;
        public double OrientationValue { get; set; } = 0.0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new Orientation
            {
                OrientationUncertainty = OrientationUncertainty,
                OrientationValue = OrientationValue
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
            var orientationUncertaintyNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}orientationUncertainty", mgr);
            if (orientationUncertaintyNode != null && orientationUncertaintyNode.HasChildNodes)
            {
                if (double.TryParse(orientationUncertaintyNode.FirstChild?.InnerText, 
                                    NumberStyles.Float, 
                                    new CultureInfo("en-US"),
                                    out double orientationUncertainty) == true) 
                {
                    OrientationUncertainty = orientationUncertainty;
                }
            }

            var orientationValueNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}orientationValue", mgr);
            if (orientationValueNode != null && orientationValueNode.HasChildNodes)
            {
                if (double.TryParse(orientationValueNode.FirstChild?.InnerText,
                                    NumberStyles.Float,
                                    new CultureInfo("en-US"),
                                    out double orientationValue) == true)
                {
                    OrientationValue = orientationValue;
                }
            }

            return this;
        }
    }
}
