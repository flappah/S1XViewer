namespace S1XViewer.Types.Interfaces
{
    public interface IMooringWarpingFacility : ILayout
    {
        string BollardDescription { get; set; }
        string BollardPull { get; set; }
        string CategoryOfMooringWarpingFacility { get; set; }
        bool HeavingLinesFromShore { get; set; }
        string IDCode { get; set; }
    }
}