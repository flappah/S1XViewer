namespace S1XViewer.Types.Interfaces
{
    public interface IQualityOfNonBathymetricData : IQualityOfTemporalVariation
    {
        string[] HorizontalDistanceUncertainty { get; set; }
        IHorizontalPositionalUncertainty HorizontalPositionalUncertainty { get; set; }
        string DirectionUncertainty { get; set; }
        ISourceIndication SourceIndication { get; set; }
        ISurveyDateRange SurveyDateRange { get; set; }
    }
}