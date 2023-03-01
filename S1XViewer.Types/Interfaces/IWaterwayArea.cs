namespace S1XViewer.Types.Interfaces
{
    public interface IWaterwayArea : IGeoFeature
    {
        string DynamicResource { get; set; }
        string SiltationRate { get; set; }
        string[] Status { get; set; }
    }
}