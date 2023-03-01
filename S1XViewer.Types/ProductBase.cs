using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public abstract class ProductBase : GeoFeatureBase, IProduct
    {
        public string Classification { get; set; }
        public string Copyright { get; set; }
        public string MaximumDisplayScale { get; set; }
        public string HorizontalDatumReference { get; set; }
        public string HorizontalDatumValue { get; set; }
        public string VerticalDatum { get; set; }
        public string SoundingDatum { get; set; }
        public string ProductType { get; set; }
        public string MinimumDisplayScale { get; set; }
        public string IssueDate { get; set; }
        public string Purpose { get; set; }

        public IInformation[] Information { get; set; }
        public IPrice[] Price { get; set; }
        public IProducingAgency ProducingAgency { get; set; }
    }
}
