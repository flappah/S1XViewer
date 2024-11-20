namespace S1XViewer.Types.Interfaces
{
    public interface IPeriodicDateRange : IComplexType
    {
        DateTime DateEnd { get; set; }
        DateTime DateStart { get; set; }

    }
}