using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface ICatalogueSectionHeader : IInformationFeature
    {
        int CatalogueSectionNumber { get; set; }
        string CatalogueSectionTitle { get; set; }
        IInformation Information { get; set; }    }
}