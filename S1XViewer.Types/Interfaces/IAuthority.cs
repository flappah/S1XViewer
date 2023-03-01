namespace S1XViewer.Types.Interfaces
{
    public interface IAuthority : IInformationFeature
    {
        string CategoryOfAuthority { get; set; }
        ITextContent TextContent { get; set; }
    }
}