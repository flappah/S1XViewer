namespace S1XViewer.Types.Interfaces
{
    public interface IPayment : IComplexType
    {
        string Currency { get; set; }
        string PriceNumber { get; set; }
    }
}
