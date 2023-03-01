namespace S1XViewer.Types.Interfaces
{
    public interface ICautionArea : IGeoFeature
    {
        string Condition { get; set; }
        string Status { get; set; }
    }
}