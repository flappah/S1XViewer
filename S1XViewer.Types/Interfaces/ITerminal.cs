namespace S1XViewer.Types.Interfaces
{
    public interface ITerminal : ILayout
    {
        string[] CategoryOfCargo { get; set; }
        string CategoryOfHarbourFacility { get; set; }
        string PortFacilityNumber { get; set; }
        string[] Product { get; set; }
        string SMDGTerminalCode { get; set; }
        string TerminalIdentifier { get; set; }
        string UNLocationCode { get; set; }
    }
}