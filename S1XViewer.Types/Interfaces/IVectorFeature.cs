using S1XViewer.Types.ComplexTypes;

namespace S1XViewer.Types.Interfaces
{
    public interface IVectorFeature
    {
        Orientation Orientation { get; set; }
        Speed Speed { get; set; }
    }
}
