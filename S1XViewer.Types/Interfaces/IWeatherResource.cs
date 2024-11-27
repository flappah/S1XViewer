using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IWeatherResource : IComplexType
    {
        string DynamicResource { get; set; }
        IOnlineResource OnlineResource { get; set; }
        ITextContent TextContent { get; set; }
    }
}