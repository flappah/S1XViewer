using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class SupportFile : ComplexTypeBase, ISupportFile
    {
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public string SupportFilePurpose { get; set; }
        public string EditionNumber { get; set; }
        public string IssueDate { get; set; }
        public ISupportFileSpecification SupportFileSpecification { get; set; }
        public string SupportFileFormat { get; set; }
        public string OtherDataTypeDescription { get; set; }
        public string Comment { get; set; }
        public string DigitalSignature { get; set; }
        public string DigitalSignatureValue { get; set; }
        public IDefaultLocale DefaultLocale { get; set; }

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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var fileNameNode = node.SelectSingleNode("fileName");
            if (fileNameNode != null && fileNameNode.HasChildNodes)
            {
                FileName = fileNameNode.FirstChild.InnerText;
            }

            var fileLocationNode = node.SelectSingleNode("fileLocation");
            if (fileLocationNode != null && fileLocationNode.HasChildNodes)
            {
                FileLocation = fileLocationNode.FirstChild.InnerText;
            }

            var supportFilePurposeNode = node.SelectSingleNode("supportFilePurpose");
            if (supportFilePurposeNode != null && supportFilePurposeNode.HasChildNodes)
            {
                SupportFilePurpose = supportFilePurposeNode.FirstChild.InnerText;
            }

            var editionNumberNode = node.SelectSingleNode("editionNumber");
            if (editionNumberNode != null && editionNumberNode.HasChildNodes)
            {
                EditionNumber = editionNumberNode.FirstChild.InnerText;
            }

            var issueDateNode = node.SelectSingleNode("issueDate");
            if (issueDateNode != null && issueDateNode.HasChildNodes)
            {
                IssueDate = issueDateNode.FirstChild.InnerText;
            }

            var supportFileSpecificationNode = node.SelectSingleNode("supportFileSpecification");
            if (supportFileSpecificationNode != null && supportFileSpecificationNode.HasChildNodes)
            {
                var supportFileSpecification = new SupportFileSpecification();
                supportFileSpecification.FromXml(supportFileSpecificationNode, mgr);
                SupportFileSpecification = supportFileSpecification;
            }

            var supportFileFormatNode = node.SelectSingleNode("supportFileFormat");
            if (supportFileFormatNode != null && supportFileFormatNode.HasChildNodes)
            {
                SupportFileFormat = supportFileFormatNode.FirstChild.InnerText;
            }

            var otherDataTypeDescriptionNode = node.SelectSingleNode("otherDataTypeDescription");
            if (otherDataTypeDescriptionNode != null && otherDataTypeDescriptionNode.HasChildNodes)
            {
                OtherDataTypeDescription = otherDataTypeDescriptionNode.FirstChild.InnerText;
            }

            var commentNode = node.SelectSingleNode("comment");
            if (commentNode != null && commentNode.HasChildNodes)
            {
                Comment = commentNode.FirstChild.InnerText;
            }

            var digitalSignatureNode = node.SelectSingleNode("digitalSignature");
            if (digitalSignatureNode != null && digitalSignatureNode.HasChildNodes)
            {
                DigitalSignature = digitalSignatureNode.FirstChild.InnerText;
            }

            var digitalSignatureValueNode = node.SelectSingleNode("digitalSignatureValue");
            if (digitalSignatureValueNode != null && digitalSignatureValueNode.HasChildNodes)
            {
                DigitalSignatureValue = digitalSignatureValueNode.FirstChild.InnerText;
            }

            var defaultLocaleNode = node.SelectSingleNode("defaultLocale");
            if (defaultLocaleNode != null && defaultLocaleNode.HasChildNodes)
            {
                var defaultLocale = new DefaultLocale();
                defaultLocale.FromXml(defaultLocaleNode, mgr);
                DefaultLocale = defaultLocale;
            }

            return this;
        }
    }
}
