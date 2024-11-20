namespace S1XViewer.Types.Interfaces
{
    public interface ITimeIntervalOfProduct : IComplexType
    {
        DateTime ExpirationDate { get; set; }
        IIssuanceCycle IssuanceCycle { get; set; }
        DateTime IssueDate { get; set; }

    }
}