namespace S1XViewer.Types.Interfaces
{
    public interface IPaperChart : IAbstractChartProduct
    {
        string FrameDimensions { get; set; }
        bool MainPanel { get; set; }
        string TypeOfPaper { get; set; }
        IPrintInformation PrintInformation { get; set; }
        IReferenceToNM ReferenceToNM { get; set; }
        string ISBN { get; set; }
    }
}