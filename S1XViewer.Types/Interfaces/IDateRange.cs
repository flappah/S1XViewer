namespace S1XViewer.Types.Interfaces
{
    public interface IDateRange : IComplexType
    {
        string EndMonthDay { get; set; }
        string StartMonthDay { get; set; }
    }
}