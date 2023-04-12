using System.Xml;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class XmlDataPackage : S1xxDataPackage, IXmlDataPackage
    {
        public XmlDocument? RawXmlData { get; set; }

    }
}
