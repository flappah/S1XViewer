using System;

namespace S1XViewer.Types.Interfaces
{
    public interface ITimeOfObservation : IComplexType
    {
        string ObservationTime { get; set; }
        string TimeReference { get; set; }
    }
}