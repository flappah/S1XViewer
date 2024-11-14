namespace S1XViewer.Types.Interfaces
{
    public interface IS104DataPackage : IBitmapDataPackage
    {
        double minX { get; set; }
        double minY { get; set; }
        double maxX { get; set; }
        double maxY { get; set; }
        double dX { get; set; }
        double dY { get; set; }
        int numPointsX { get; set; }
        int numPointsY { get; set; }
        float[,] Data { get; set; }
    }
}
