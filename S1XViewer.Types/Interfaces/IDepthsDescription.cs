using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IDepthsDescription : IComplexType
    {
        string CategoryOfDepthsDescription { get; set; }
        ITextContent[] TextContent { get; set; }
    }
}