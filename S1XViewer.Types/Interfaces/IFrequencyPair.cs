namespace S1XViewer.Types.Interfaces
{
    public interface IFrequencyPair : IComplexType
    {
        string[] FrequencyShoreStationReceives { get; set; }
        string[] FrequencyShoreStationTransmits { get; set; }
        string[] ContactInstructions { get; set; }
    }
}