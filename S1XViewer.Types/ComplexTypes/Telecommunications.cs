using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Telecommunications : ComplexTypeBase, ITelecommunications
    {
        public string CategoryOfCommPref { get; set; } = string.Empty;
        public string TelecommunicationIdentifier { get; set; } = string.Empty;
        public string TelcomCarrier { get; set; } = string.Empty;
        public string ContactInstructions { get; set; } = string.Empty;
        public string[] TelecommunicationService { get; set; } = Array.Empty<string>();
        public IScheduleByDoW[] ScheduleByDoW { get; set; } = Array.Empty<ScheduleByDoW>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new Telecommunications
            {
                CategoryOfCommPref = CategoryOfCommPref,
                ContactInstructions = ContactInstructions,
                ScheduleByDoW = ScheduleByDoW == null   
                    ? Array.Empty<ScheduleByDoW>()
                    : Array.ConvertAll(ScheduleByDoW, s => s.DeepClone() as IScheduleByDoW),
                TelcomCarrier = TelcomCarrier,
                TelecommunicationIdentifier = TelecommunicationIdentifier,
                TelecommunicationService = TelecommunicationService
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
            var telecommunicationIdentifierNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}telecommunicationIdentifier", mgr);
            if (telecommunicationIdentifierNode != null && telecommunicationIdentifierNode.HasChildNodes)
            {
                TelecommunicationIdentifier = telecommunicationIdentifierNode.FirstChild?.InnerText ?? string.Empty;
            }

            var telecommunicationsServiceNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}telecommunicationService", mgr);
            if (telecommunicationsServiceNodes != null && telecommunicationsServiceNodes.Count > 0)
            {
                var services = new List<string>();
                foreach (XmlNode telecommunicationsServiceNode in telecommunicationsServiceNodes)
                {
                    if (telecommunicationsServiceNode != null && telecommunicationsServiceNode.HasChildNodes)
                    {
                        services.Add(telecommunicationsServiceNode.InnerText);
                    }
                }
                TelecommunicationService = services.ToArray();
            }

            var categoryOfCommPrefNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfCommPref", mgr);
            if (categoryOfCommPrefNode != null && categoryOfCommPrefNode.HasChildNodes)
            {
                CategoryOfCommPref = categoryOfCommPrefNode.FirstChild?.InnerText ?? string.Empty;
            }

            var contactInstructionsNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}contactInstructions", mgr);
            if (contactInstructionsNode != null && contactInstructionsNode.HasChildNodes)
            {
                ContactInstructions = contactInstructionsNode.FirstChild?.InnerText ?? string.Empty;
            }

            var telcomCarrierNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}telcomCarrier", mgr);
            if (telcomCarrierNode != null && telcomCarrierNode.HasChildNodes)
            {
                TelcomCarrier = telcomCarrierNode.FirstChild?.InnerText ?? string.Empty;
            }

            var scheduleByDoWNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}scheduleByDoW", mgr);
            if (scheduleByDoWNodes != null && scheduleByDoWNodes.Count > 0)
            {
                var nodes = new List<ScheduleByDoW>();
                foreach(XmlNode scheduleByDoWNode in scheduleByDoWNodes)
                {
                    if (scheduleByDoWNode != null && scheduleByDoWNode.HasChildNodes)
                    {
                        var scheduleByDoW = new ScheduleByDoW();
                        scheduleByDoW.FromXml(scheduleByDoWNode, mgr, nameSpacePrefix);
                        nodes.Add(scheduleByDoW);
                    }
                }
                ScheduleByDoW = nodes.ToArray();
            }

            return this;
        }
    }
}
