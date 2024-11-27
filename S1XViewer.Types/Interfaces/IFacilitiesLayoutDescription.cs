using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IFacilitiesLayoutDescription : IComplexType
    {
        ITextContent[] TextContent { get; set; }
    }
}