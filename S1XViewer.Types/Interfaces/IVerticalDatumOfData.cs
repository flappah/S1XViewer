using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IVerticalDatumOfData
    {
        IInformation[] Information { get; set; }
        string VerticalDatum { get; set; }

        IFeature DeepClone();
        IFeature FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}