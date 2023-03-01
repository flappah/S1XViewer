using System.Data;
using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IFeature
    {
        IFeatureObjectIdentifier FeatureObjectIdentifier { get; set; }
        string Id { get; set; }

        ILink[] Links { get; set; }

        void Clear();
        IFeature DeepClone();
        DataTable GetData();
        IFeature FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}
