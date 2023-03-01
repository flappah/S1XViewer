using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface ILink
    {
        string Href { get; set; }
        string ArcRole { get; set; }
        string Name { get; set; }
        string Offset { get; set; }
        IFeature LinkedFeature { get; set; }

        ILink DeepClone();
        ILink FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}
