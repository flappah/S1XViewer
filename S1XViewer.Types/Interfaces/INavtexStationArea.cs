namespace S1XViewer.Types.Interfaces
{
    public interface INavtexStationArea : IGeoFeature
    {
        string Restriction { get; set; }
        string Status { get; set; }
        string TxIdentChar { get; set; }
    }
}