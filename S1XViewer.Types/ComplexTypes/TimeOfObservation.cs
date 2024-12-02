using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class TimeOfObservation : ComplexTypeBase, ITimeOfObservation
    {
        public string ObservationTime { get; set; } = string.Empty;
        public string TimeReference { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new TimeOfObservation
            {
                ObservationTime = ObservationTime,
                TimeReference = TimeReference
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
            var observationTimeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}observationTime", mgr);
            if (observationTimeNode != null && observationTimeNode.HasChildNodes)
            {
                ObservationTime = observationTimeNode.FirstChild?.InnerText ?? string.Empty;
            }

            var timeReferenceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}timeReference", mgr);
            if (timeReferenceNode != null && timeReferenceNode.HasChildNodes)
            {
                TimeReference = timeReferenceNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
