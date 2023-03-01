namespace S1XViewer.Types.Interfaces
{
    public interface IAbstractRxN : IInformationFeature
    {
        string CategoryOfAuthority { get; set; }
        IGraphic[] Graphic { get; set; }
        IRxnCode[] RxnCode { get; set; }
        ITextContent[] TextContent { get; set; }
    }
}
