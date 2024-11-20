using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface ICatalogueElement
    {
        string AgencyResponsibleForProduction { get; set; }
        string[] CatalogueElementClassification { get; set; }
        string CatalogueElementIdentifier { get; set; }
        string Classification { get; set; }
        string[] IMOMaritimeService { get; set; }
        IInformation[] Information { get; set; }
        string Keywords { get; set; }
        bool NotForNavigation { get; set; }
        IOnlineResource OnlineResource { get; set; }
        ISupportFile[] SupportFile { get; set; }

        IFeature FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}