using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IFeatureFactory
    {
        IFeature FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}
