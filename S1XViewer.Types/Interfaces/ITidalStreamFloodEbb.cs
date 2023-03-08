using S1XViewer.Types.ComplexTypes;

namespace S1XViewer.Types.Interfaces
{
    public interface ITidalStreamFloodEbb : IGeoFeature
    {
        string[] CategoryOfTidalStream { get; set; }
        Orientation Orientation { get; set; }
        Speed Speed { get; set; }
    }
}
