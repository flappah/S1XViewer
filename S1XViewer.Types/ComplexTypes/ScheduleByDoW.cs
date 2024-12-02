using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class ScheduleByDoW : ComplexTypeBase, IScheduleByDoW
    {
        public string CategoryOfSchedule { get; set; } = string.Empty;
        public ITmIntervalsByDoW[] TmIntervalsByDoW { get; set; } = Array.Empty<TmIntervalsByDoW>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new ScheduleByDoW
            {
                CategoryOfSchedule = CategoryOfSchedule,
                TmIntervalsByDoW = TmIntervalsByDoW == null
                    ? Array.Empty<TmIntervalsByDoW>()
                    : Array.ConvertAll(TmIntervalsByDoW, t => t.DeepClone() as ITmIntervalsByDoW)
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
            var categoryOfScheduleNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfSchedule", mgr);
            if (categoryOfScheduleNode != null && categoryOfScheduleNode.HasChildNodes)
            {
                CategoryOfSchedule = categoryOfScheduleNode.FirstChild?.InnerText ?? string.Empty;
            }

            var tmIntervalsByDoWNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}tmIntervalsByDoW", mgr);
            if (tmIntervalsByDoWNodes != null && tmIntervalsByDoWNodes.Count > 0)
            {
                var intervals = new List<TmIntervalsByDoW>();
                foreach(XmlNode tmIntervalsByDoWNode in tmIntervalsByDoWNodes)
                {
                    if (tmIntervalsByDoWNode != null && tmIntervalsByDoWNode.HasChildNodes)
                    {
                        var newInterval = new TmIntervalsByDoW();
                        newInterval.FromXml(tmIntervalsByDoWNode, mgr, nameSpacePrefix);
                        intervals.Add(newInterval);
                    }
                }
                TmIntervalsByDoW = intervals.ToArray();
            }

            return this;
        }
    }
}
