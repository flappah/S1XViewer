namespace S1XViewer.Types.Interfaces
{
    public interface IBitmapDataPackage : IHdfDataPackage
    {
        double noDataValue { get; set; }
        string TiffFileName { get; set; }
        double minDataValue { get; set; }   
        double maxDataValue { get; set; }
    }
}
