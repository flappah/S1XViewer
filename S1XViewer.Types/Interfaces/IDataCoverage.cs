namespace S1XViewer.Types.Interfaces
{
    public interface IDataCoverage : IMetaFeature
    {
        string MaximumDisplayScale { get; set; }
        string MinimumDisplayScale { get; set; }
    }
}