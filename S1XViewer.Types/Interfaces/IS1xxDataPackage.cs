using Esri.ArcGISRuntime.Geometry;

namespace S1XViewer.Types.Interfaces
{
    public interface IS1xxDataPackage
    {
        Guid Id { get; set; }
        public string FileName { get; set; }
        S1xxTypes Type { get; set; }
        IMetaFeature[] MetaFeatures { get; set; }
        IInformationFeature[] InformationFeatures { get; set; }
        IGeoFeature[] GeoFeatures { get; set; }
        Geometry BoundingBox { get; set; }
    }
}