namespace S1XViewer.Types.Interfaces
{
    public interface ISupportFile : IComplexType
    {
        string FileName { get; set; }
        string FileLocation { get; set; }
        string SupportFilePurpose { get; set; }
        string EditionNumber { get; set; }
        string IssueDate { get; set; }
        ISupportFileSpecification SupportFileSpecification { get; set; }
        string SupportFileFormat { get; set; }
        string OtherDataTypeDescription { get; set; }
        string Comment { get; set; }
        string DigitalSignature { get; set; }
        string DigitalSignatureValue { get; set; }
        IDefaultLocale DefaultLocale { get; set; }
    }
}
