using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class TmIntervalsByDoW : ComplexTypeBase, ITmIntervalsByDoW
    {
        public string[] DayOfWeek { get; set; } = Array.Empty<string>();
        public string DayOfWeekIsRanges { get; set; } = string.Empty;
        public string TimeReference { get; set; } = string.Empty;
        public string[] TimeOfDayStart { get; set; } = Array.Empty<string>();
        public string[] TimeOfDayEnd { get; set; } = Array.Empty<string>();

        /// <summary>
        ///     Deep clones the object
        /// </summary>
        /// <returns>IComplexType</returns>
        public override IComplexType DeepClone()
        {
            return new TmIntervalsByDoW
            {
                DayOfWeek = DayOfWeek == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(DayOfWeek, t => t),
                DayOfWeekIsRanges = DayOfWeekIsRanges,
                TimeReference = TimeReference,
                TimeOfDayEnd = TimeOfDayEnd == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(TimeOfDayEnd, t => t),
                TimeOfDayStart = TimeOfDayStart == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(TimeOfDayStart, t => t)
            };
        }

        /// <summary>
        ///     Reads the data from an XML dom
        /// </summary>
        /// <param name="node">current node to use as a starting point for reading</param>
        /// <param name="mgr">xml namespace manager</param>
        /// <returns>IFeature</returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var dayOfWeekNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}dayOfWeek", mgr);
            if (dayOfWeekNodes != null && dayOfWeekNodes.Count > 0)
            {
                var days = new List<string>();
                foreach (XmlNode dayOfWeekNode in dayOfWeekNodes)
                {
                    if (dayOfWeekNode != null && dayOfWeekNode.HasChildNodes)
                    {
                        string dayOfWeekText = dayOfWeekNode.FirstChild?.InnerText ?? string.Empty;
                        days.Add(dayOfWeekText);
                    }
                }

                DayOfWeek = days.ToArray();
            }

            var dayOfWeekIsRangesNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}dayOfWeekIsRanges", mgr);
            if (dayOfWeekIsRangesNode != null && dayOfWeekIsRangesNode.HasChildNodes)
            {
                DayOfWeekIsRanges = dayOfWeekIsRangesNode.FirstChild?.InnerText ?? string.Empty;
            }

            var timeReferenceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}timeReference", mgr);
            if (timeReferenceNode != null && timeReferenceNode.HasChildNodes)
            {
                TimeReference = timeReferenceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var timeOfDayStartNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}timeOfDayStart", mgr);
            if (timeOfDayStartNodes != null && timeOfDayStartNodes.Count > 0)
            {
                var times = new List<string>();
                foreach (XmlNode timeOfDayStartNode in timeOfDayStartNodes)
                {
                    if (timeOfDayStartNode != null && timeOfDayStartNode.HasChildNodes)
                    {
                        string timeOfDayStart = timeOfDayStartNode.FirstChild?.InnerText ?? string.Empty;
                        times.Add(timeOfDayStart);
                    }
                }
                TimeOfDayStart = times.ToArray(); 
            }

            var timeOfDayEndNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}timeOfDayEnd", mgr);
            if (timeOfDayEndNodes != null && timeOfDayEndNodes.Count > 0)
            {
                var times = new List<string>();
                foreach (XmlNode timeOfDayEndNode in timeOfDayEndNodes)
                {
                    if (timeOfDayEndNode != null && timeOfDayEndNode.HasChildNodes)
                    {
                        string timeOfDayEnd = timeOfDayEndNode.FirstChild?.InnerText ?? string.Empty;
                        times.Add(timeOfDayEnd);
                    }
                }
                TimeOfDayEnd = times.ToArray(); 
            }

            return this;
        }
    }
}
