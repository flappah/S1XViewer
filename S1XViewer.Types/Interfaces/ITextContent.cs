namespace S1XViewer.Types.Interfaces
{
    public interface ITextContent : IComplexType
    {
        string CategoryOfText { get; set; }
        IInformation[] Information { get; set; }
        IOnlineResource OnlineResource { get; set; }
        ISourceIndication SourceIndication { get; set; }
    }
}