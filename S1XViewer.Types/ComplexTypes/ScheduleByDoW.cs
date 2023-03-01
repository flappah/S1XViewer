using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class ScheduleByDoW : ComplexTypeBase, IScheduleByDoW
    {
        public string CategoryOfSchedule { get; set; }
        public ITmIntervalsByDoW[] TmIntervalsByDoW { get; set; }

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
                    ? new TmIntervalsByDoW[0]
                    : Array.ConvertAll(TmIntervalsByDoW, t => t.DeepClone() as ITmIntervalsByDoW)
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
            var categoryOfScheduleNode = node.SelectSingleNode("categoryOfSchedule", mgr);
            if (categoryOfScheduleNode != null && categoryOfScheduleNode.HasChildNodes)
            {
                CategoryOfSchedule = categoryOfScheduleNode.FirstChild.InnerText;
            }

            var tmIntervalsByDoWNodes = node.SelectNodes("tmIntervalsByDoW", mgr);
            if (tmIntervalsByDoWNodes != null && tmIntervalsByDoWNodes.Count > 0)
            {
                var intervals = new List<TmIntervalsByDoW>();
                foreach(XmlNode tmIntervalsByDoWNode in tmIntervalsByDoWNodes)
                {
                    if (tmIntervalsByDoWNode != null && tmIntervalsByDoWNode.HasChildNodes)
                    {
                        var newInterval = new TmIntervalsByDoW();
                        newInterval.FromXml(tmIntervalsByDoWNode, mgr);
                        intervals.Add(newInterval);
                    }
                }
                TmIntervalsByDoW = intervals.ToArray();
            }

            return this;
        }
    }
}
