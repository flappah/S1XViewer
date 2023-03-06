namespace S1XViewer.HDF.Interfaces
{
    public interface IProductSupportFactory
    {
        IProductSupportBase[] Supports { get; set; }

        IProductSupportBase Create(string productStandard);
    }
}