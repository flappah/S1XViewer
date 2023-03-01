using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Features
{
    public abstract class DataQuality : MetaFeatureBase, IDataQuality
    {
        public IInformation[] Information { get; set; }
    }
}
