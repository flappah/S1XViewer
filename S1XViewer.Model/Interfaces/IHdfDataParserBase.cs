using HDF5CSharp.DataTypes;

namespace S1XViewer.Model.Interfaces
{
    public interface IHdfDataParserBase : IDataParser
    {
        Hdf5Element RetrieveHdf5File(string hdf5FileName);
        Task<Hdf5Element> RetrieveHdf5FileAsync(string hdf5FileName);
        Task<(DateTime start, DateTime end)> RetrieveTimeFrameFromHdfDatasetAsync(string hdf5FileName);
    }
}
