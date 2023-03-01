namespace S1XViewer.Types.Interfaces
{
    public interface ITelecommunications : IComplexType
    {
        string CategoryOfCommPref { get; set; }
        string ContactInstructions { get; set; }
        string TelcomCarrier { get; set; }
        string TelecommunicationIdentifier { get; set; }
        string[] TelecommunicationService { get; set; }
        IScheduleByDoW[] ScheduleByDoW { get; set; }
    }
}