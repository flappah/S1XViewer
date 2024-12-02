using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IHarbourAreaSection : ILayout
    {
        string[] CategoryOfHarbourFacility { get; set; }
        string CategoryOfPortSection { get; set; }
        string FacilitiesLayoutDescription { get; set; }
        string iSPSLevel { get; set; }
    }
}