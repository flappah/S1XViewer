namespace S1XViewer.HDF.Interfaces
{
    public interface IDatasetReader
    {
        IEnumerable<T> Read<T>(string fileName, string name);
    }
}
