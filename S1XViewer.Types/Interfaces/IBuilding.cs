using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IBuilding : IGeoFeature
    {
        string[] Function { get; set; }
        string[] Status { get; set; }
    }
}