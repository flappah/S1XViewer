namespace S1XViewer.Types.Interfaces
{
    public interface ISounding : IGeoFeature
    {
        float Value { get; set; }
        string[] QualityOfVerticalMeasurement { get; set; }
        DateTime ReportedDate { get; set; }
        string Status { get; set; }
        string[] TechniqueOfVerticalMeasurement { get; set; }
        IVerticalUncertainty? VerticalUncertainty { get; set; }
    }
}