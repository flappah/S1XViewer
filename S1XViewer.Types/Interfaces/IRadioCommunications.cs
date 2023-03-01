using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Interfaces
{
    public interface IRadioCommunications : IComplexType
    {
        string CategoryOfCommPref { get; set; }
        string[] CategoryOfMaritimeBroadcast { get; set; }
        string[] CategoryOfRadioMethods { get; set; }
        string[] CommunicationChannel { get; set; }
        string ContactInstructions { get; set; }
        IFacsimileDrumSpeed FacsimileDrumSpeed { get; set; }
        IFrequencyPair[] FrequencyPair { get; set; }
        string SelectiveCallNumber { get; set; }
        string SignalFrequency { get; set; }
        ITimeOfObservation TimeOfObservation { get; set; }
        ITimesOfTransmission TimesOfTransmission { get; set; }
        string TransmissionContent { get; set; }
        ITmIntervalsByDoW[] TmIntervalsByDoW { get; set; }
        string[] TransmissionRegularity { get; set; }
    }
}