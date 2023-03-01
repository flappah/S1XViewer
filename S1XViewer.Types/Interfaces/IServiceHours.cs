using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Interfaces
{
    public interface IServiceHours : IInformationFeature
    {
        IScheduleByDoW[] ScheduleByDoW { get; set; }
        IInformation[] Information { get; set; }
    }
}