namespace S1XViewer.Types.Interfaces
{
    public interface IAbstractChartProduct : ICatalogueElements
    {
        string ChartNumber { get; set; }
        string[] CompilationScale { get; set; }
        string DistributionStatus { get; set; }
        string ProducerCode { get; set; }
        string ProducerNation { get; set; }
        string SpecificUsage { get; set; }
    }
}
