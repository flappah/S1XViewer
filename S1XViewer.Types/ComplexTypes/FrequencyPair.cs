using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class FrequencyPair : ComplexTypeBase, IFrequencyPair
    {
        public string[] FrequencyShoreStationReceives { get; set; }
        public string[] FrequencyShoreStationTransmits { get; set; }
        public string[] ContactInstructions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new FrequencyPair
            {
                FrequencyShoreStationReceives = FrequencyShoreStationReceives == null
                    ? new string[0]
                    : Array.ConvertAll(FrequencyShoreStationReceives, i => i),
                FrequencyShoreStationTransmits = FrequencyShoreStationTransmits == null
                    ? new string[0]
                    : Array.ConvertAll(FrequencyShoreStationTransmits, i => i),
                ContactInstructions = ContactInstructions == null
                    ? new string[0]
                    : Array.ConvertAll(ContactInstructions, s => s)
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
            var frequencyShoreStationReceivesNodes = node.SelectNodes("frequencyShoreStationReceives");
            if (frequencyShoreStationReceivesNodes != null && frequencyShoreStationReceivesNodes.Count > 0)
            {
                var frequencies = new List<string>();
                foreach(XmlNode frequencyShoreStationReceivesNode in frequencyShoreStationReceivesNodes)
                {
                    if (frequencyShoreStationReceivesNode != null && frequencyShoreStationReceivesNode.HasChildNodes)
                    {
                        frequencies.Add(frequencyShoreStationReceivesNode.FirstChild.InnerText);
                    }
                }

                FrequencyShoreStationReceives = frequencies.ToArray();
            }

            var frequencyShoreStationTransmitsNodes = node.SelectNodes("frequencyShoreStationTransmits");
            if (frequencyShoreStationTransmitsNodes != null && frequencyShoreStationTransmitsNodes.Count > 0)
            {
                var frequencies = new List<string>();
                foreach (XmlNode frequencyShoreStationTransmitsNode in frequencyShoreStationTransmitsNodes)
                {
                    if (frequencyShoreStationTransmitsNode != null && frequencyShoreStationTransmitsNode.HasChildNodes)
                    {
                        frequencies.Add(frequencyShoreStationTransmitsNode.FirstChild.InnerText);
                    }
                }

                FrequencyShoreStationTransmits = frequencies.ToArray();
            }

            return this;
        }
    }
}
