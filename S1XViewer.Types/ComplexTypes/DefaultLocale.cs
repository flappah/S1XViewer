using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class DefaultLocale : ComplexTypeBase, IDefaultLocale
    {
        public string Language { get; set; } = string.Empty;
        public string CharacterEncoding { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var dateNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}language", mgr);
            if (dateNode != null && dateNode.HasChildNodes)
            {
                Language = dateNode.FirstChild?.InnerText ?? string.Empty;
            }

            var characterEncodingValueNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}characterEncoding", mgr);
            if (characterEncodingValueNode != null && characterEncodingValueNode.HasChildNodes)
            {
                CharacterEncoding = characterEncodingValueNode.FirstChild?.InnerText ?? string.Empty;
            }

            var countryValueNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}country", mgr);
            if (countryValueNode != null && countryValueNode.HasChildNodes)
            {
                Country = countryValueNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
