using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IS100Service : ICatalogueElement
    {
        bool CompressionFlag { get; set; }
        string EncodingFormat { get; set; }
        IProductSpecification ProductSpecification { get; set; }
        string ServiceName { get; set; }
        IServiceSpecification ServiceSpecification { get; set; }
        string ServiceStatus { get; set; }
        string TypeOfProductFormat { get; set; }

        IFeature DeepClone();
        IFeature FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}