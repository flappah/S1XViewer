namespace S1XViewer.Types.Interfaces
{
    public interface IChartProduct : IProduct
    {
        string ChartNumber { get; set; }
        string CompilationScale { get; set; }
        string DistributionStatus { get; set; }
        string EditionNumber { get; set; }
        string ProducerCode { get; set; }
        string ProducerNation { get; set; }
        string SpecificUsage { get; set; }
    }
}