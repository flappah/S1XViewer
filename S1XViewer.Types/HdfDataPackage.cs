using HDF5CSharp.DataTypes;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class HdfDataPackage : S1xxDataPackage, IHdfDataPackage
    {

        public Hdf5Element? RawHdfData { get; set; }
    }
}
