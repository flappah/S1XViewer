namespace S1XViewer.Types.Interfaces
{
    public interface ITidalStation : IGeoFeature
    {
        Dictionary<DateTime, float> TidalHeights { get; set; } 
        Dictionary<DateTime, short> TidalTrends { get; set; } 
        short SelectedIndex { get; set; }
        string SelectedDateTime { get; set; }
        string SelectedHeight { get; set; }
        string SelectedTrend { get; set; }
    }
}
