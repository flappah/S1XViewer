namespace S1XViewer.Types.Interfaces
{
    public interface ISupportFileSpecification : IComplexType
    {
        string Date { get; set; }
        string Name { get; set; }
        string Version { get; set; }
    }
}
