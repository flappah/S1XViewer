using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class DateRange : ComplexTypeBase, IDateRange
    {
        public string StartMonthDay { get; set; }
        public string EndMonthDay { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node != null && node.HasChildNodes)
            {
                var dateStart = node.SelectSingleNode("dateStart", mgr);
                if (dateStart != null && dateStart.HasChildNodes)
                {
                    StartMonthDay = dateStart.FirstChild.InnerText;
                }

                var dateEnd = node.SelectSingleNode("dateEnd", mgr);
                if (dateEnd != null && dateEnd.HasChildNodes)
                {
                    EndMonthDay = dateEnd.FirstChild.InnerText;
                }
            }

            return this;
        }
    }
}
