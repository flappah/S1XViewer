namespace S1XViewer.Types.Interfaces
{
    public interface IRadioStation : IGeoFeature
    {
        string CallSign { get; set; }
        string CategoryOfRadioStation { get; set; }
        string EstimatedRangeOffTransmission { get; set; }
        IOrientation Orientation { get; set; }
        IRadioStationCommunicationDescription[] RadioStationCommunicationDescription { get; set; }
        string Status { get; set; }
    }
}