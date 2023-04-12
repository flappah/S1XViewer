using HDF5CSharp.DataTypes;

namespace S1XViewer.Types.Interfaces
{
    public interface IHdfDataPackage : IS1xxDataPackage
    {
        Hdf5Element? RawHdfData { get; set; }
    }
}