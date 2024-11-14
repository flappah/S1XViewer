using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S102DataPackage : HdfDataPackage, IS102DataPackage
    {   
        public double minX {  get; set; }   
        public double minY { get; set; }
        public double maxX { get; set; }
        public double maxY { get; set; }
        public double dX { get; set; }
        public double dY { get; set; }
        public double noDataValue { get; set; }
        public int numPointsX { get; set; }
        public int numPointsY { get; set; }
        public float[,] Data { get; set; } = new float[0, 0];
        public string TiffFileName { get; set; } = string.Empty;
        public double minDataValue { get; set; }
        public double maxDataValue { get; set; }

        public S102DataPackage()
        {
            Id = Guid.NewGuid();
        }
    }
}
