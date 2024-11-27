namespace S1XViewer.Types.Interfaces
{
    public interface ILandmarkDescription : IComplexType
    {
        ITextContent[] TextContent { get; set; }
    }
}