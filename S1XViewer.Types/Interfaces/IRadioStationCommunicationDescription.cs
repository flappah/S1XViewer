namespace S1XViewer.Types.Interfaces
{
    public interface IRadioStationCommunicationDescription : IComplexType
    {
        string[] CategoryOfMaritimeBroadcast { get; set; }
        string[] CommunicationChannel { get; set; }
        string SignalFrequency { get; set; }
        string TransmissionContent { get; set; }
    }
}