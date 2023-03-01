namespace S1XViewer.Types.Interfaces
{
    public interface IPriceInformation : IInformationFeature
    {
        IPayment Payment { get; set; }
    }
}
