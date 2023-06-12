namespace S1XViewer.Types.Interfaces
{
    public interface ITidalStation : IGeoFeature
    {
        Dictionary<string, string> TidalHeights { get; set; } 
        Dictionary<string, string> TidalTrends { get; set; } 
        short SelectedIndex { get; set; }
        string SelectedDateTime { get; set; }
        string SelectedHeight { get; set; }
        string SelectedTrend { get; set; }
    }
}
