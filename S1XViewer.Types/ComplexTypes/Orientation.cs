using S1XViewer.Types.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Orientation : ComplexTypeBase, IOrientation
    {
        public double OrientationUncertainty { get; set; }
        public double OrientationValue { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var orientationUncertaintyNode = node.SelectSingleNode("orientationUncertainty", mgr);
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

            var orientationValueNode = node.SelectSingleNode("orientationValue", mgr);
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
