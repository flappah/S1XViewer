using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IXmlDataPackage : IS1xxDataPackage
    {
        XmlDocument? RawXmlData { get; set; }
    }
}