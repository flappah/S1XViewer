using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IMajorLightDescription : IComplexType
    {
        ITextContent[] TextContent { get; set; }
    }
}