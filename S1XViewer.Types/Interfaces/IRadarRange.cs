using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IRadarRange : IGeoFeature
    {
        string[] Status { get; set; }
    }
}