namespace S1XViewer.Types.Interfaces
{
    public interface IBearingInformation : IComplexType
     {
        string CardinalDirection { get; set; }
        string Distance { get; set; }
        IInformation[] Information { get; set; }
        IOrientation Orientation { get; set; }
        string[] SectorBearing { get; set; }
    }
}