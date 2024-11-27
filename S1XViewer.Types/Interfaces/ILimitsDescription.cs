using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface ILimitsDescription : IComplexType
    {
        ITextContent[] TextContent { get; set; }
    }
}