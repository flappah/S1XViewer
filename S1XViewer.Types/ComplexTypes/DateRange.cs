using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class DateRange : ComplexTypeBase, IDateRange
    {
        public string StartMonthDay { get; set; } = string.Empty;
        public string EndMonthDay { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new DateRange
            {
                StartMonthDay = StartMonthDay,
                EndMonthDay = EndMonthDay
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node != null && node.HasChildNodes)
            {
                var dateStart = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}dateStart", mgr);
                if (dateStart != null && dateStart.HasChildNodes)
                {
                    StartMonthDay = dateStart.FirstChild?.InnerText ?? string.Empty;
                }

                var dateEnd = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}dateEnd", mgr);
                if (dateEnd != null && dateEnd.HasChildNodes)
                {
                    EndMonthDay = dateEnd.FirstChild?.InnerText ?? string.Empty;
                }
            }

            return this;
        }
    }
}
