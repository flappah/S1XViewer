using System.Xml;
using Esri.ArcGISRuntime.Geometry;

namespace S1XViewer.Types.Interfaces
{
    public interface IS1xxDataPackage
    {
        S1xxTypes Type { get; set; }
        IMetaFeature[] MetaFeatures { get; set; }
        IInformationFeature[] InformationFeatures { get; set; }
        IGeoFeature[] GeoFeatures { get; set; }
        Geometry BoundingBox { get; set; }
        XmlDocument RawData { get; set; }
    }
}