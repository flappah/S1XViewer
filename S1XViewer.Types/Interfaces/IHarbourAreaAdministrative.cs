using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IHarbourAreaAdministrative : ILayout
    {
        string ApplicableLoadLineZone { get; set; }
        string[] CategoryOfHarbourFacility { get; set; }
        IGeneralHarbourInformation GeneralHarbourInformation { get; set; }
        string ISPSLevel { get; set; }
        string Nationality { get; set; }
        string UNLocationCode { get; set; }
    }
}