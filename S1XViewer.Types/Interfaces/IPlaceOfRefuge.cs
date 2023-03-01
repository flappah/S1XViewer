using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IPlaceOfRefuge : IGeoFeature
    {
        string[] Status { get; set; }
    }
}