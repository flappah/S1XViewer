namespace S1XViewer.Types.Interfaces
{
    public interface IVerticalUncertainty : IComplexType
    {
        double UncertaintyFixed { get; set; }
        double UncertaintyVariable { get; set; }
    }
}