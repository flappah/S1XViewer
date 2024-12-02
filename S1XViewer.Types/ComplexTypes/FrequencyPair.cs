using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class FrequencyPair : ComplexTypeBase, IFrequencyPair
    {
        public string[] FrequencyShoreStationReceives { get; set; } = Array.Empty<string>();
        public string[] FrequencyShoreStationTransmits { get; set; } = Array.Empty<string>();
        public string[] ContactInstructions { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new FrequencyPair
            {
                FrequencyShoreStationReceives = FrequencyShoreStationReceives == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(FrequencyShoreStationReceives, i => i),
                FrequencyShoreStationTransmits = FrequencyShoreStationTransmits == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(FrequencyShoreStationTransmits, i => i),
                ContactInstructions = ContactInstructions == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(ContactInstructions, s => s)
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
            var frequencyShoreStationReceivesNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}frequencyShoreStationReceives", mgr);
            if (frequencyShoreStationReceivesNodes != null && frequencyShoreStationReceivesNodes.Count > 0)
            {
                var frequencies = new List<string>();
                foreach(XmlNode frequencyShoreStationReceivesNode in frequencyShoreStationReceivesNodes)
                {
                    if (frequencyShoreStationReceivesNode != null && frequencyShoreStationReceivesNode.HasChildNodes)
                    {
                        frequencies.Add(frequencyShoreStationReceivesNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                FrequencyShoreStationReceives = frequencies.ToArray();
            }

            var frequencyShoreStationTransmitsNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}frequencyShoreStationTransmits", mgr);
            if (frequencyShoreStationTransmitsNodes != null && frequencyShoreStationTransmitsNodes.Count > 0)
            {
                var frequencies = new List<string>();
                foreach (XmlNode frequencyShoreStationTransmitsNode in frequencyShoreStationTransmitsNodes)
                {
                    if (frequencyShoreStationTransmitsNode != null && frequencyShoreStationTransmitsNode.HasChildNodes)
                    {
                        frequencies.Add(frequencyShoreStationTransmitsNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                FrequencyShoreStationTransmits = frequencies.ToArray();
            }

            return this;
        }
    }
}
