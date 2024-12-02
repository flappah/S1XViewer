using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class TimesOfTransmission : ComplexTypeBase, ITimesOfTransmission
    {
        public string MinutePastEvenHours { get; set; } = string.Empty;
        public string MinutePastEveryHours { get; set; } = string.Empty;
        public string MinutePastOddHours { get; set; } = string.Empty;
        public string TimeReference { get; set; } = string.Empty;
        public string[] TransmissionTime { get; set; } = Array.Empty<string>();

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
                    ? Array.Empty<string>()
                    : Array.ConvertAll(TransmissionTime, s => s)
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
            var minutePastEvenHoursNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}minutePastEvenHours", mgr);
            if (minutePastEvenHoursNode != null && minutePastEvenHoursNode.HasChildNodes)
            {
                MinutePastEvenHours = minutePastEvenHoursNode.FirstChild?.InnerText ?? string.Empty;
            }

            var minutePastEveryHoursNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}minutePastEveryHours", mgr);
            if (minutePastEveryHoursNode != null && minutePastEveryHoursNode.HasChildNodes)
            {
                MinutePastEveryHours = minutePastEveryHoursNode.FirstChild?.InnerText ?? string.Empty;
            }

            var minutePastOddHoursNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}minutePastOddHours", mgr);
            if (minutePastOddHoursNode != null && minutePastOddHoursNode.HasChildNodes)
            {
                MinutePastOddHours = minutePastOddHoursNode.FirstChild?.InnerText ?? string.Empty;
            }

            var timeReferenceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}timeReference", mgr);
            if (timeReferenceNode != null && timeReferenceNode.HasChildNodes)
            {
                TimeReference = timeReferenceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var transmissionTimeNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}transmissionTime", mgr);
            if (transmissionTimeNodes != null && transmissionTimeNodes.Count > 0)
            {
                var transmissionTimes = new List<string>();
                foreach(XmlNode transmissionTimeNode in transmissionTimeNodes)
                {
                    if (transmissionTimeNode != null && transmissionTimeNode.HasChildNodes)
                    {
                        transmissionTimes.Add(transmissionTimeNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                TransmissionTime = transmissionTimes.ToArray();
            }

            return this;
        }
    }
}
