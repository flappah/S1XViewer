using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class FacsimileDrumSpeed : ComplexTypeBase, IFacsimileDrumSpeed
    {
        public string DrumSpeed { get; set; } = string.Empty;
        public string IndexOfCooperation { get; set; } = string.Empty;

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var drumSpeedNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}drumSpeed", mgr);
            if (drumSpeedNode != null && drumSpeedNode.HasChildNodes)
            {
                DrumSpeed = drumSpeedNode.FirstChild?.InnerText ?? string.Empty;
            }

            var indexOfCooperationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}indexOfCooperation", mgr);
            if (indexOfCooperationNode != null && indexOfCooperationNode.HasChildNodes)
            {
                IndexOfCooperation = indexOfCooperationNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
