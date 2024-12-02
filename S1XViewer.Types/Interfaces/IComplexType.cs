using System.Data;
using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IComplexType
    {
        IComplexType DeepClone();
        IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "");
        DataTable GetData();
    }
}
