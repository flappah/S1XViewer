using System;

namespace S1XViewer.Types.Interfaces
{
    public interface IProduct : IGeoFeature
    {
        string Classification { get; set; }
        string Copyright { get; set; }
        string HorizontalDatumReference { get; set; }
        string HorizontalDatumValue { get; set; }
        IInformation[] Information { get; set; }
        string IssueDate { get; set; }
        string MaximumDisplayScale { get; set; }
        string MinimumDisplayScale { get; set; }
        IPrice[] Price { get; set; }
        IProducingAgency ProducingAgency { get; set; }
        string ProductType { get; set; }
        string Purpose { get; set; }
        string SoundingDatum { get; set; }
        string VerticalDatum { get; set; }
    }
}