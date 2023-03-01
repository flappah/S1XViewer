using S1XViewer.Types.ComplexTypes;

namespace S1XViewer.Types.Interfaces
{
    public interface IInformation : IComplexType
    {
        string FileReference { get; set; }
        string FileLocator { get; set; }
        string Headline { get; set; }
        string Language { get; set; }
        string Text { get; set; }
    }
}