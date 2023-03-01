namespace S1XViewer.Types.Interfaces
{
    public interface IContactDetails : IInformationFeature
    {
        string CallName { get; set; }
        string CallSign { get; set; }
        string CategoryOfCommPref { get; set; }
        string[] CommunicationChannel { get; set; }
        string ContactInstructions { get; set; }
        string MMsiCode { get; set; }

        IContactAddress[] ContactAddress { get; set; }
        IFrequencyPair[] FrequencyPair { get; set; }
        IOnlineResource[] OnlineResource { get; set; }
        IRadioCommunications[] RadioCommunications { get; set; }
        ITelecommunications[] Telecommunications { get; set; }

    }
}