using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Telecommunications : ComplexTypeBase, ITelecommunications
    {
        public string CategoryOfCommPref { get; set; }
        public string TelecommunicationIdentifier { get; set; }
        public string TelcomCarrier { get; set; }
        public string ContactInstructions { get; set; }
        public string[] TelecommunicationService { get; set; }
        public IScheduleByDoW[] ScheduleByDoW { get; set; }

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
                    ? new ScheduleByDoW[0]
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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var telecommunicationIdentifierNode = node.SelectSingleNode("telecommunicationIdentifier", mgr);
            if (telecommunicationIdentifierNode != null && telecommunicationIdentifierNode.HasChildNodes)
            {
                TelecommunicationIdentifier = telecommunicationIdentifierNode.FirstChild.InnerText;
            }

            var telecommunicationsServiceNodes = node.SelectNodes("telecommunicationService", mgr);
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

            var categoryOfCommPrefNode = node.SelectSingleNode("categoryOfCommPref", mgr);
            if (categoryOfCommPrefNode != null && categoryOfCommPrefNode.HasChildNodes)
            {
                CategoryOfCommPref = categoryOfCommPrefNode.FirstChild.InnerText;
            }

            var contactInstructionsNode = node.SelectSingleNode("contactInstructions", mgr);
            if (contactInstructionsNode != null && contactInstructionsNode.HasChildNodes)
            {
                ContactInstructions = contactInstructionsNode.FirstChild.InnerText;
            }

            var telcomCarrierNode = node.SelectSingleNode("telcomCarrier", mgr);
            if (telcomCarrierNode != null && telcomCarrierNode.HasChildNodes)
            {
                TelcomCarrier = telcomCarrierNode.FirstChild.InnerText;
            }

            var scheduleByDoWNodes = node.SelectNodes("scheduleByDoW", mgr);
            if (scheduleByDoWNodes != null && scheduleByDoWNodes.Count > 0)
            {
                var nodes = new List<ScheduleByDoW>();
                foreach(XmlNode scheduleByDoWNode in scheduleByDoWNodes)
                {
                    if (scheduleByDoWNode != null && scheduleByDoWNode.HasChildNodes)
                    {
                        var scheduleByDoW = new ScheduleByDoW();
                        scheduleByDoW.FromXml(scheduleByDoWNode, mgr);
                        nodes.Add(scheduleByDoW);
                    }
                }
                ScheduleByDoW = nodes.ToArray();
            }

            return this;
        }
    }
}
