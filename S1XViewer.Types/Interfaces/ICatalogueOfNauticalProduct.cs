namespace S1XViewer.Types.Interfaces
{
    public interface ICatalogueOfNauticalProduct : IGeoFeature
    {
        string EditionNumber { get; set; }
        IGraphic[] Graphic { get; set; }
        string IssueDate { get; set; }
        string MarineResourceName { get; set; }
    }
}