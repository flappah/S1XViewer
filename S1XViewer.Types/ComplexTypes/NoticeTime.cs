using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class NoticeTime : ComplexTypeBase, INoticeTime
    {
        public string[] NoticeTimeHours { get; set; } = Array.Empty<string>();
        public string NoticeTimeText { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new NoticeTime
            {
                NoticeTimeHours = NoticeTimeHours == null 
                ? Array.Empty<string>()
                : Array.ConvertAll(NoticeTimeHours, n => n),
                NoticeTimeText = NoticeTimeText ?? "",
                Operation= Operation
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
            var noticeTimeHoursNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}noticeTimeHours", mgr);
            if (noticeTimeHoursNodes != null && noticeTimeHoursNodes.Count > 0)
            {
                var noticeHours = new List<string>();
                foreach (XmlNode noticeTimeHoursNode in noticeTimeHoursNodes)
                {
                    if (noticeTimeHoursNode != null && noticeTimeHoursNode.HasChildNodes)
                    {
                        noticeHours.Add(noticeTimeHoursNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                NoticeTimeHours = noticeHours.ToArray();
            }

            var noticeTimeTextNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}noticeTimeText", mgr);
            if (noticeTimeTextNode != null && noticeTimeTextNode.HasChildNodes)
            {
                NoticeTimeText = noticeTimeTextNode.FirstChild?.InnerText ?? string.Empty;
            }

            var operationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}operation", mgr);
            if (operationNode != null && operationNode.HasChildNodes)
            {
                Operation = operationNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
