namespace S1XViewer.Types.Interfaces
{
    public interface IConcentrationOfShippingHazardArea : IGeoFeature
    {
        string[] CategoryOfConcentrationOfShippingHazardArea { get; set; }
        string[] Status { get; set; }
    }
}