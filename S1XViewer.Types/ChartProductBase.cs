using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public abstract class ChartProductBase : ProductBase, IChartProduct
    {
        public string ChartNumber { get; set; }
        public string DistributionStatus { get; set; }
        public string CompilationScale { get; set; }
        public string EditionNumber { get; set; }
        public string SpecificUsage { get; set; }
        public string ProducerCode { get; set; }
        public string ProducerNation { get; set; }
    }
}
