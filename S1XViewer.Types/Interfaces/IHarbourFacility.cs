using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IHarbourFacility : IHarbourPhysicalInfrastructure
    {
        string[] CategoryOfHarbourFacility { get; set; }
    }
}