using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface ICargoServicesDescription : IComplexType
    {
        ITextContent[] TextContent { get; set; }

    }
}