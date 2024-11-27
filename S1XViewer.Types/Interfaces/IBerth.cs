using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IBerth : ILayout
    {
        float AvailableBerthingLength { get; set; }
        string BollardDescription { get; set; }
        string[] BollardNumber { get; set; }
        float BollardPull { get; set; }
        string CategoryOfBerthLocation { get; set; }
        bool CathodicProtectionSystem { get; set; }
        float Elevation { get; set; }
        string GLNExtension { get; set; }
        string LocationByText { get; set; }
        string[] ManifoldNumber { get; set; }
        string MethodOfSecuring { get; set; }
        string[] MetreMarkNumber { get; set; }
        float MinimumBerthDepth { get; set; }
        string PortFacilityNumber { get; set; }
        string RampNumber { get; set; }
        string TerminalIdentifier { get; set; }
        string UNLocationCode { get; set; }

    }
}