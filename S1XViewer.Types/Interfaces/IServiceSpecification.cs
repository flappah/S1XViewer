namespace S1XViewer.Types.Interfaces
{
    public interface IServiceSpecification : IComplexType
    {
        string Date { get; set; }
        string Name { get; set; }
        string Version { get; set; }
    }
}
