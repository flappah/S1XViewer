namespace S1XViewer.Types.Interfaces
{
    public interface ICatalogueElements
    {
        string Classification { get; set; }
        string Copyright { get; set; }
        string EditionDate { get; set; }
        string EditionNumber { get; set; }
        string HorizontalDatumReference { get; set; }
        string IssueDate { get; set; }
        string MarineResourceName { get; set; }
        string MaximumDisplayScale { get; set; }
        string MinimumDisplayScale { get; set; }
        string ProductType { get; set; }
        string Purpose { get; set; }
        string SoundingDatum { get; set; }
        string UpdateDate { get; set; }
        string UpdateNumber { get; set; }
        string VerticalDatum { get; set; }
        IGraphic[] Graphic { get; set; }
        IInformation[] Information { get; set; }
        IPayment[] Payment { get; set; }
        IProducingAgency ProducingAgency { get; set; }
        ISupportFile[] SupportFile { get; set; }
    }
}
