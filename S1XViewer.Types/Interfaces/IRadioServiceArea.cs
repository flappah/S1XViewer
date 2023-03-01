namespace S1XViewer.Types.Interfaces
{
    public interface IRadioServiceArea : IGeoFeature
    {
        string CallSign { get; set; }
        string CategoryOfBroadcastCommunication { get; set; }
        string LanguageInformation { get; set; }
        IRadioCommunications[] RadioCommunications { get; set; }
        string Status { get; set; }
        string TimeReference { get; set; }
        string TransmissionPower { get; set; }
        string TxIdentChar { get; set; }
        string TxTrafficList { get; set; }
    }
}