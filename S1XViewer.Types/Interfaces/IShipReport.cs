namespace S1XViewer.Types.Interfaces
{
    public interface IShipReport : IInformationFeature
    {
        string[] CategoryOfShipReport { get; set; }
        string ImoFormatForReporting { get; set; }
        INoticeTime[] NoticeTime { get; set; }
        ITextContent TextContent { get; set; }
    }
}