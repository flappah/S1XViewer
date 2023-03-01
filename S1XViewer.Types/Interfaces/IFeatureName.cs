namespace S1XViewer.Types.Interfaces
{
    public interface IFeatureName : IComplexType
    {
        string DisplayName { get; set; }
        string Language { get; set; }
        string Name { get; set; }
    }
}