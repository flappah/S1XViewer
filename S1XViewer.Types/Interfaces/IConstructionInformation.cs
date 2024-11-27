using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IConstructionInformation : IComplexType
    {
        string Condition { get; set; }
        string Development { get; set; }
        IDateRange FixedDateRange { get; set; }
        string LocationByText { get; set; }
        ITextContent[] TextContent { get; set; }
    }
}