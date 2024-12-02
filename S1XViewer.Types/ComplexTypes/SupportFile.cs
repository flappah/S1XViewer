using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class SupportFile : ComplexTypeBase, ISupportFile
    {
        public string FileName { get; set; } = string.Empty;
        public string FileLocation { get; set; } = string.Empty;
        public string SupportFilePurpose { get; set; } = string.Empty;
        public string EditionNumber { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;
        public ISupportFileSpecification SupportFileSpecification { get; set; } = new SupportFileSpecification();
        public string SupportFileFormat { get; set; } = string.Empty;
        public string OtherDataTypeDescription { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string DigitalSignature { get; set; } = string.Empty;
        public string DigitalSignatureValue { get; set; } = string.Empty;
        public IDefaultLocale DefaultLocale { get; set; } = new DefaultLocale();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new SupportFile
            {
                FileName = FileName,
                FileLocation = FileLocation,
                SupportFilePurpose = SupportFilePurpose,
                EditionNumber = EditionNumber,
                IssueDate = IssueDate,
                SupportFileSpecification = SupportFileSpecification == null
                    ? null
                    : SupportFileSpecification.DeepClone() as ISupportFileSpecification,
                SupportFileFormat = SupportFileFormat,
                OtherDataTypeDescription = OtherDataTypeDescription,
                Comment = Comment,
                DigitalSignature = DigitalSignature,
                DigitalSignatureValue = DigitalSignatureValue,
                DefaultLocale = DefaultLocale == null
                    ? null
                    : DefaultLocale.DeepClone() as IDefaultLocale
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
            var fileNameNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}fileName", mgr);
            if (fileNameNode != null && fileNameNode.HasChildNodes)
            {
                FileName = fileNameNode.FirstChild?.InnerText ?? string.Empty;
            }

            var fileLocationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}fileLocation", mgr);
            if (fileLocationNode != null && fileLocationNode.HasChildNodes)
            {
                FileLocation = fileLocationNode.FirstChild?.InnerText ?? string.Empty;
            }

            var supportFilePurposeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}supportFilePurpose", mgr);
            if (supportFilePurposeNode != null && supportFilePurposeNode.HasChildNodes)
            {
                SupportFilePurpose = supportFilePurposeNode.FirstChild?.InnerText ?? string.Empty;
            }

            var editionNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}editionNumber", mgr);
            if (editionNumberNode != null && editionNumberNode.HasChildNodes)
            {
                EditionNumber = editionNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            var issueDateNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}issueDate", mgr);
            if (issueDateNode != null && issueDateNode.HasChildNodes)
            {
                IssueDate = issueDateNode.FirstChild?.InnerText ?? string.Empty;
            }

            var supportFileSpecificationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}supportFileSpecification", mgr);
            if (supportFileSpecificationNode != null && supportFileSpecificationNode.HasChildNodes)
            {
                var supportFileSpecification = new SupportFileSpecification();
                supportFileSpecification.FromXml(supportFileSpecificationNode, mgr, nameSpacePrefix);
                SupportFileSpecification = supportFileSpecification;
            }

            var supportFileFormatNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}supportFileFormat", mgr);
            if (supportFileFormatNode != null && supportFileFormatNode.HasChildNodes)
            {
                SupportFileFormat = supportFileFormatNode.FirstChild?.InnerText ?? string.Empty;
            }

            var otherDataTypeDescriptionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}otherDataTypeDescription", mgr);
            if (otherDataTypeDescriptionNode != null && otherDataTypeDescriptionNode.HasChildNodes)
            {
                OtherDataTypeDescription = otherDataTypeDescriptionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var commentNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}comment", mgr);
            if (commentNode != null && commentNode.HasChildNodes)
            {
                Comment = commentNode.FirstChild?.InnerText ?? string.Empty;
            }

            var digitalSignatureNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}digitalSignature", mgr);
            if (digitalSignatureNode != null && digitalSignatureNode.HasChildNodes)
            {
                DigitalSignature = digitalSignatureNode.FirstChild?.InnerText ?? string.Empty;
            }

            var digitalSignatureValueNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}digitalSignatureValue", mgr);
            if (digitalSignatureValueNode != null && digitalSignatureValueNode.HasChildNodes)
            {
                DigitalSignatureValue = digitalSignatureValueNode.FirstChild?.InnerText ?? string.Empty;
            }

            var defaultLocaleNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}defaultLocale", mgr);
            if (defaultLocaleNode != null && defaultLocaleNode.HasChildNodes)
            {
                var defaultLocale = new DefaultLocale();
                defaultLocale.FromXml(defaultLocaleNode, mgr, nameSpacePrefix);
                DefaultLocale = defaultLocale;
            }

            return this;
        }
    }
}
