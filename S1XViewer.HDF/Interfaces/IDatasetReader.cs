namespace S1XViewer.HDF.Interfaces
{
    public interface IDatasetReader
    {
        IEnumerable<T> Read<T>(string fileName, string name);
        float[,] ReadArrayOfFloats(string fileName, string name, int dim1, int dim2);
    }
}
