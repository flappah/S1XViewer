using S1XViewer.Types.Interfaces;
using System.Globalization;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Speed : ComplexTypeBase, ISpeed
    {
        public double SpeedMaximum { get; set; } = 0.0;
        public double SpeedMinimum { get; set; } = 0.0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new Speed
            {
                SpeedMaximum = SpeedMaximum,
                SpeedMinimum = SpeedMinimum
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
            var speedMaximumNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}speedMaximum", mgr);
            if (speedMaximumNode != null && speedMaximumNode.HasChildNodes)
            {
                if (double.TryParse(speedMaximumNode.FirstChild?.InnerText, 
                                    NumberStyles.Float, 
                                    new CultureInfo("en-US"), 
                                    out double speedMaximum) == true)
                {
                    SpeedMaximum = speedMaximum;
                }
            }

            var speedMinimumNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}speedMinimum", mgr);
            if (speedMinimumNode != null && speedMinimumNode.HasChildNodes)
            {
                if (double.TryParse(speedMinimumNode.FirstChild?.InnerText,
                                    NumberStyles.Float,
                                    new CultureInfo("en-US"),
                                    out double speedMinimum) == true)
                {
                    SpeedMinimum = speedMinimum;
                }
            }

            return this;
        }
    }
}
