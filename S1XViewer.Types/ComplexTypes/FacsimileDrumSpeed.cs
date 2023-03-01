using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class FacsimileDrumSpeed : ComplexTypeBase, IFacsimileDrumSpeed
    {
        public string DrumSpeed { get; set; }
        public string IndexOfCooperation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new FacsimileDrumSpeed
            {
                DrumSpeed = DrumSpeed,
                IndexOfCooperation = IndexOfCooperation
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
            var drumSpeedNode = node.SelectSingleNode("drumSpeed");
            if (drumSpeedNode != null && drumSpeedNode.HasChildNodes)
            {
                DrumSpeed = drumSpeedNode.FirstChild.InnerText;
            }

            var indexOfCooperationNode = node.SelectSingleNode("indexOfCooperation");
            if (indexOfCooperationNode != null && indexOfCooperationNode.HasChildNodes)
            {
                IndexOfCooperation = indexOfCooperationNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
