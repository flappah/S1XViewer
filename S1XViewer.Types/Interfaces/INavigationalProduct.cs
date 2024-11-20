using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface INavigationalProduct : ICatalogueElement
    {
        int ApproximateGridResolution { get; set; }
        int[] CompilationScale { get; set; }
        string DistributionStatus { get; set; }
        string[] NavigationPurpose { get; set; }
        int OptimumDisplayScale { get; set; }
        string OriginalProductNumber { get; set; }
        string ProducerNation { get; set; }
        string ProductNumber { get; set; }
        string SpecificUsage { get; set; }
        string UpdateDate { get; set; }
        string UpdateNumber { get; set; }

        IFeature FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}