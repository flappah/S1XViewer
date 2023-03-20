using HDF5CSharp.DataTypes;
using System.Runtime.InteropServices;

namespace S1XViewer.HDF
{
    public struct BathymetryCoverageInformationInstance
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
        [Hdf5EntryName("code")]
        public string code;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        [Hdf5EntryName("name")]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        [Hdf5EntryName("uom.name")]
        public string uomName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        [Hdf5EntryName("fillValue")]
        public string fillValue;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        [Hdf5EntryName("dataType")]
        public string dataType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        [Hdf5EntryName("lower")]
        public string lower;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        [Hdf5EntryName("upper")]
        public string upper;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        [Hdf5EntryName("closure")]
        public string closure;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="uomName"></param>
        /// <param name="fillValue"></param>
        /// <param name="dataType"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <param name="closure"></param>
        public BathymetryCoverageInformationInstance(string code, string name, string uomName, string fillValue, string dataType, string lower, string upper, string closure)
        {
            this.code = code;
            this.name = name;
            this.uomName = uomName;
            this.fillValue = fillValue;
            this.dataType = dataType;
            this.lower = lower;
            this.upper = upper;
            this.closure = closure;
        }
    }
}
