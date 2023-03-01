namespace S1XViewer.Types.Interfaces
{
    public interface IRxnCode : IComplexType
    {
        string ActionOrActivity { get; set; }
        string CategoryOfRxn { get; set; }
        string Headline { get; set; }
    }
}