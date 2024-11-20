namespace S1XViewer.Types.Interfaces
{
    public interface ITimeIntervalOfCycle : IComplexType
    {
        int ValueOfTime { get; set; }
        string TypeOfTimeIntervalUnit { get; set; }

    }
}