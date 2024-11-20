using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface ICatalogueSectionHeader
    {
        int CatalogueSectionNumber { get; set; }
        string CatalogueSectionTitle { get; set; }
        IInformation Information { get; set; }

        IFeature DeepClone();
        IFeature FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}