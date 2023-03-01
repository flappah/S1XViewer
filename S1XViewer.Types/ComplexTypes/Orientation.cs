using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Orientation : ComplexTypeBase, IOrientation
    {
        public string OrientationUncertainty { get; set; }
        public string OrientationValue { get; set; }

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
                OrientationUncertainty = orientationUncertaintyNode.FirstChild.InnerText;
            }

            var orientationValueNode = node.SelectSingleNode("orientationValue", mgr);
            if (orientationValueNode != null && orientationValueNode.HasChildNodes)
            {
                OrientationValue = orientationValueNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
