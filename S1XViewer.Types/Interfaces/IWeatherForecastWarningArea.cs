namespace S1XViewer.Types.Interfaces
{
    public interface IWeatherForecastWarningArea : IGeoFeature
    {
        string CategoryOfFrctAndWarningArea { get; set; }
        string Nationality { get; set; }
        string Status { get; set; }
    }
}