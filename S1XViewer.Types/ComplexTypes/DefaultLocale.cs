using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class DefaultLocale : ComplexTypeBase, IDefaultLocale
    {
        public string Language { get; set; }
        public string CharacterEncoding { get; set; }
        public string Country { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new DefaultLocale
            {
                Language = Language,
                CharacterEncoding = CharacterEncoding,
                Country = Country
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
            var dateNode = node.SelectSingleNode("language");
            if (dateNode != null && dateNode.HasChildNodes)
            {
                Language = dateNode.FirstChild.InnerText;
            }

            var characterEncodingValueNode = node.SelectSingleNode("characterEncoding");
            if (characterEncodingValueNode != null && characterEncodingValueNode.HasChildNodes)
            {
                CharacterEncoding = characterEncodingValueNode.FirstChild.InnerText;
            }

            var countryValueNode = node.SelectSingleNode("country");
            if (countryValueNode != null && countryValueNode.HasChildNodes)
            {
                Country = countryValueNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
