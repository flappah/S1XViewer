using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IBerthPosition : ILayout
    {
        float AvailableBerthingLength { get; set; }
        string BollardDescription { get; set; }
        string[] BollardNumber { get; set; }
        float BollardPull { get; set; }
        string GLNExtension { get; set; }
        string LocationByText { get; set; }
        string[] ManifoldNumber { get; set; }
        string[] MetreMarkNumber { get; set; }
        string RampNumber { get; set; }

    }
}