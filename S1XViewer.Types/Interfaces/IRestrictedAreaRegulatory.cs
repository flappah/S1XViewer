namespace S1XViewer.Types.Interfaces
{
    public interface IRestrictedAreaRegulatory : IGeoFeature
    {
        string[] CategoryOfRestrictedArea { get; set; }
        string[] Restriction { get; set; }
        string[] Status { get; set; }
    }
}