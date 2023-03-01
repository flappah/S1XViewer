using System.Xml;

namespace S1XViewer.Base.Interfaces
{
    public interface IInjectableXmlDocument
    {
        XmlElement GetElementById(string elementId);
        XmlNodeList GetElementsByTagName(string name);
        XmlDocument Load(string fileName);
        XmlDocument LoadXml(string xml);
        XmlNodeList SelectNodes(string xpath);
        XmlNode SelectSingleNode(string xpath);
    }
}