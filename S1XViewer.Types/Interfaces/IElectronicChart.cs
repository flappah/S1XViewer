namespace S1XViewer.Types.Interfaces
{
    public interface IElectronicChart : IAbstractChartProduct
    {
        IProductSpecification ProductSpecification { get; set; }
        string[] DatasetName { get; set; }
        string TnpUpdate { get; set; }
        string TypeOfProductFormat { get; set; }
    }
}