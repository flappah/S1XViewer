using Esri.ArcGISRuntime.Geometry;
using HDF5CSharp.DataTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types
{
    public class S1xxDataPackage : IS1xxDataPackage
    {
        public S1xxTypes Type { get; set; }
        public IMetaFeature[] MetaFeatures { get; set; }
        public IInformationFeature[] InformationFeatures { get; set; }
        public IGeoFeature[] GeoFeatures { get; set; }
        public Geometry BoundingBox { get; set; }
        public XmlDocument? RawXmlData { get; set; }
        public Hdf5Element? RawHdfData { get; set; }
    }
}
