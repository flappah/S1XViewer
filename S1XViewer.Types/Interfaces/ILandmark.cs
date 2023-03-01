namespace S1XViewer.Types.Interfaces
{
    public interface ILandmark : IGeoFeature
    {
        string[] CategoryOfLandmark { get; set; }
        string[] Function { get; set; }
        string[] Status { get; set; }

    }
}