using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IVerticalDatumOfData : IMetaFeature
    {
        IInformation[] Information { get; set; }
        string VerticalDatum { get; set; }
    }
}