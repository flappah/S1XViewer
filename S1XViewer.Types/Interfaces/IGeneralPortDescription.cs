using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IGeneralPortDescription : IComplexType
    {
        ITextContent[] TextContent { get; set; }
    }
}