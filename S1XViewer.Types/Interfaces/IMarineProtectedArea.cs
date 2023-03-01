using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Interfaces
{
    public interface IMarineProtectedArea : IGeoFeature
    {
        string CategoryOfMarineProtectedArea { get; set; }
        string[] CategoryOfRestrictedArea { get; set; }
        string Jurisdiction { get; set; }
        string[] Restriction { get; set; }
        string[] Status { get; set; }
    }
}