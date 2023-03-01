using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class TmIntervalsByDoW : ComplexTypeBase, ITmIntervalsByDoW
    {
        public string[] DayOfWeek { get; set; }
        public string DayOfWeekIsRanges { get; set; }
        public string TimeReference { get; set; }
        public string[] TimeOfDayStart { get; set; }
        public string[] TimeOfDayEnd { get; set; }

        /// <summary>
        ///     Deep clones the object
        /// </summary>
        /// <returns>IComplexType</returns>
        public override IComplexType DeepClone()
        {
            return new TmIntervalsByDoW
            {
                DayOfWeek = DayOfWeek == null
                    ? new string[0]
                    : Array.ConvertAll(DayOfWeek, t => t),
                DayOfWeekIsRanges = DayOfWeekIsRanges,
                TimeReference = TimeReference,
                TimeOfDayEnd = TimeOfDayEnd == null
                    ? new string[0]
                    : Array.ConvertAll(TimeOfDayEnd, t => t),
                TimeOfDayStart = TimeOfDayStart == null
                    ? new string[0]
                    : Array.ConvertAll(TimeOfDayStart, t => t)
            };
        }

        /// <summary>
        ///     Reads the data from an XML dom
        /// </summary>
        /// <param name="node">current node to use as a starting point for reading</param>
        /// <param name="mgr">xml namespace manager</param>
        /// <returns>IFeature</returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var dayOfWeekNodes = node.SelectNodes("dayOfWeek");
            if (dayOfWeekNodes != null && dayOfWeekNodes.Count > 0)
            {
                var days = new List<string>();
                foreach (XmlNode dayOfWeekNode in dayOfWeekNodes)
                {
                    if (dayOfWeekNode != null && dayOfWeekNode.HasChildNodes)
                    {
                        string dayOfWeekText = dayOfWeekNode.FirstChild.InnerText;
                        days.Add(dayOfWeekText);
                    }
                }

                DayOfWeek = days.ToArray();
            }

            var dayOfWeekIsRangesNode = node.SelectSingleNode("dayOfWeekIsRanges", mgr);
            if (dayOfWeekIsRangesNode != null && dayOfWeekIsRangesNode.HasChildNodes)
            {
                DayOfWeekIsRanges = dayOfWeekIsRangesNode.FirstChild.InnerText;
            }

            var timeReferenceNode = node.SelectSingleNode("timeReference", mgr);
            if (timeReferenceNode != null && timeReferenceNode.HasChildNodes)
            {
                TimeReference = timeReferenceNode.FirstChild.InnerText;
            }

            var timeOfDayStartNodes = node.SelectNodes("timeOfDayStart", mgr);
            if (timeOfDayStartNodes != null && timeOfDayStartNodes.Count > 0)
            {
                var times = new List<string>();
                foreach (XmlNode timeOfDayStartNode in timeOfDayStartNodes)
                {
                    if (timeOfDayStartNode != null && timeOfDayStartNode.HasChildNodes)
                    {
                        string timeOfDayStart = timeOfDayStartNode.FirstChild.InnerText;
                        times.Add(timeOfDayStart);
                    }
                }
                TimeOfDayStart = times.ToArray(); 
            }

            var timeOfDayEndNodes = node.SelectNodes("timeOfDayEnd", mgr);
            if (timeOfDayEndNodes != null && timeOfDayEndNodes.Count > 0)
            {
                var times = new List<string>();
                foreach (XmlNode timeOfDayEndNode in timeOfDayEndNodes)
                {
                    if (timeOfDayEndNode != null && timeOfDayEndNode.HasChildNodes)
                    {
                        string timeOfDayEnd = timeOfDayEndNode.FirstChild.InnerText;
                        times.Add(timeOfDayEnd);
                    }
                }
                TimeOfDayEnd = times.ToArray(); 
            }

            return this;
        }
    }
}
