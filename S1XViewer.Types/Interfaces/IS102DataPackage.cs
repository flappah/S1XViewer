namespace S1XViewer.Types.Interfaces
{
    public interface IS102DataPackage : IHdfDataPackage
    {
        double minX { get; set; }
        double minY { get; set; }
        double maxX { get; set; }
        double maxY { get; set; }
        double dX { get; set; }
        double dY { get; set; }
        double noDataValue { get; set; }
        int numPointsX { get; set; }
        int numPointsY { get; set; }
        float[,] Data { get; set; } 
        string TiffFileName { get; set; }
    }
}