namespace S1XViewer.Types.Interfaces
{
    public interface IElectronicProduct : INavigationalProduct
    {
        bool CompressionFlag { get; set; }
        string DatasetName { get; set; }
        string EncodingFormat { get; set; }
        DateTime IssueDateTime { get; set; }
        IProductSpecification ProductSpecification { get; set; }
        string TypeOfProductFormat { get; set; }
    }
}