using System;

namespace S1XViewer.Types.Interfaces
{
    public interface ITimesOfTransmission : IComplexType
    {
        string MinutePastEvenHours { get; set; }
        string MinutePastEveryHours { get; set; }
        string MinutePastOddHours { get; set; }
        string TimeReference { get; set; }
        string[] TransmissionTime { get; set; }
    }
}