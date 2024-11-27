namespace S1XViewer.Types.Interfaces
{
    public interface ISoundingDatum : IMetaFeature
    {
        IInformation[] Information { get; set; }
        string VerticalDatum { get; set; }
    }
}