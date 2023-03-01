using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Model.Interfaces
{
    public interface IDataParser
    {
        delegate void ProgressFunction(double percentage);
        event ProgressFunction? Progress;

        Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument);
        IS1xxDataPackage Parse(XmlDocument xmlDocument);
    }
}
