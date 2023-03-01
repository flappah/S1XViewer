using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IPilotBoardingPlace : IGeoFeature
    {
        string CallSign { get; set; }
        string CategoryOfPilotBoardingPlace { get; set; }
        string CategoryOfPreference { get; set; }
        string CategoryOfVessel { get; set; }
        string[] CommunicationChannel { get; set; }
        string Destination { get; set; }
        string PilotMovement { get; set; }
        string PilotVessel { get; set; }
        string[] Status { get; set; }
    }
}