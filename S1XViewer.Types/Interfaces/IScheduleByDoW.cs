using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Interfaces
{
    public interface IScheduleByDoW : IComplexType
    {
        string CategoryOfSchedule { get; set; }
        ITmIntervalsByDoW[] TmIntervalsByDoW { get; set; }
    }
}