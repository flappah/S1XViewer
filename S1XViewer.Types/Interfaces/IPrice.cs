namespace S1XViewer.Types.Interfaces
{
    public interface IPrice : IComplexType
    {
        string Currency { get; set; }
        string PriceNumber { get; set; }
    }
}