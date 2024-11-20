using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class TimeIntervalOfCycle : ComplexTypeBase, ITimeIntervalOfCycle
    {
        public string TypeOfTimeIntervalUnit { get; set; } = string.Empty;
        public int ValueOfTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new TimeIntervalOfCycle()
            {
                TypeOfTimeIntervalUnit = TypeOfTimeIntervalUnit,
                ValueOfTime = ValueOfTime
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
            //public string TypeOfTimeIntervalUnit { get; set; } = string.Empty;
            var typeOfTimeIntervalUnitNode = node.SelectSingleNode("typeOfTimeIntervalUnit", mgr);
            if (typeOfTimeIntervalUnitNode == null)
            {
                typeOfTimeIntervalUnitNode = node.SelectSingleNode("S128:typeOfTimeIntervalUnit", mgr);
            }
            if (typeOfTimeIntervalUnitNode != null && typeOfTimeIntervalUnitNode.HasChildNodes)
            {
                TypeOfTimeIntervalUnit = typeOfTimeIntervalUnitNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public int ValueOfTime { get; set; }
            var valueOfTimeNode = node.SelectSingleNode("valueOfTime", mgr);
            if (valueOfTimeNode == null)
            {
                valueOfTimeNode = node.SelectSingleNode("S128:valueOfTime", mgr);
            }
            if (valueOfTimeNode != null && valueOfTimeNode.HasChildNodes)
            {
                if (int.TryParse(valueOfTimeNode.FirstChild?.InnerText, out int valueOfTimeValue))
                {
                    ValueOfTime = valueOfTimeValue;
                }
            }

            return this;
        }
    }
}
