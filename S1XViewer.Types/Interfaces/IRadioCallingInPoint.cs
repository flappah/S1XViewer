namespace S1XViewer.Types.Interfaces
{
    public interface IRadioCallingInPoint : IGeoFeature
    {
        string CallSign { get; set; }
        string[] CategoryOfCargo { get; set; }
        string CategoryOfVessel { get; set; }
        string[] CommunicationChannel { get; set; }
        IOrientation[] Orientation { get; set; }
        string[] Status { get; set; }
        string TrafficFlow { get; set; }
    }
}