namespace S1XViewer.HDF.Interfaces
{
    public interface IDatasetReader
    {
        IEnumerable<T> Read<T>(string fileName, string name);
        IEnumerable<string> ReadStrings(string fileName, string name);
        (string[] members, float[,] values) ReadArrayOfFloats(string fileName, string name, int dim1, int dim2);
        public (string[] members, T[] values) ReadCompound<T>(string fileName, string name) where T : struct;
    }
}
