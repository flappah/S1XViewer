using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class TimesOfTransmission : ComplexTypeBase, ITimesOfTransmission
    {
        public string MinutePastEvenHours { get; set; }
        public string MinutePastEveryHours { get; set; }
        public string MinutePastOddHours { get; set; }
        public string TimeReference { get; set; }
        public string[] TransmissionTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new TimesOfTransmission
            {
                MinutePastEvenHours = MinutePastEvenHours,
                MinutePastEveryHours = MinutePastEveryHours,
                MinutePastOddHours = MinutePastOddHours,
                TimeReference = TimeReference,
                TransmissionTime = TransmissionTime == null
                    ? new string[0]
                    : Array.ConvertAll(TransmissionTime, s => s)
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
            var minutePastEvenHoursNode = node.SelectSingleNode("minutePastEvenHours");
            if (minutePastEvenHoursNode != null && minutePastEvenHoursNode.HasChildNodes)
            {
                MinutePastEvenHours = minutePastEvenHoursNode.FirstChild.InnerText;
            }

            var minutePastEveryHoursNode = node.SelectSingleNode("minutePastEveryHours");
            if (minutePastEveryHoursNode != null && minutePastEveryHoursNode.HasChildNodes)
            {
                MinutePastEveryHours = minutePastEveryHoursNode.FirstChild.InnerText;
            }

            var minutePastOddHoursNode = node.SelectSingleNode("minutePastOddHours");
            if (minutePastOddHoursNode != null && minutePastOddHoursNode.HasChildNodes)
            {
                MinutePastOddHours = minutePastOddHoursNode.FirstChild.InnerText;
            }

            var timeReferenceNode = node.SelectSingleNode("timeReference");
            if (timeReferenceNode != null && timeReferenceNode.HasChildNodes)
            {
                TimeReference = timeReferenceNode.FirstChild.InnerText;
            }

            var transmissionTimeNodes = node.SelectNodes("transmissionTime");
            if (transmissionTimeNodes != null && transmissionTimeNodes.Count > 0)
            {
                var transmissionTimes = new List<string>();
                foreach(XmlNode transmissionTimeNode in transmissionTimeNodes)
                {
                    if (transmissionTimeNode != null && transmissionTimeNode.HasChildNodes)
                    {
                        transmissionTimes.Add(transmissionTimeNode.FirstChild.InnerText);
                    }
                }
                TransmissionTime = transmissionTimes.ToArray();
            }

            return this;
        }
    }
}
