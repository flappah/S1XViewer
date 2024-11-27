using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public abstract class HarbourPhysicalInfrastructure : SupervisedArea, IHarbourPhysicalInfrastructure, IS131Feature
    {
        public float VerticalClearance { get; set; } = 0.0f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            base.FromXml(node, mgr);

            var verticalClearanceValueNode = node.SelectSingleNode("verticalClearanceValue", mgr);
            if (verticalClearanceValueNode != null && verticalClearanceValueNode.HasChildNodes)
            {
                if (float.TryParse(verticalClearanceValueNode.FirstChild?.InnerText, out float verticalClearanceValue))
                {
                    VerticalClearance = verticalClearanceValue;
                }
            }

            return this;
        }
    }
}
