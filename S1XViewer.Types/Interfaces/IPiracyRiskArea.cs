namespace S1XViewer.Types.Interfaces
{
    public interface IPiracyRiskArea : IGeoFeature
    {
        string[] Restriction { get; set; }
        string[] Status { get; set; }
    }
}