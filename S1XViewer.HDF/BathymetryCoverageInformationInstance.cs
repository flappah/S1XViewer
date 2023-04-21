using PureHDF;
using System.Runtime.InteropServices;

namespace S1XViewer.HDF
{
    public struct BathymetryCoverageInformationInstance
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 25)]
        [H5Name("code")]
        public string code;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        [H5Name("name")]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        [H5Name("uom.name")]
        public string uomName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        [H5Name("fillValue")]
        public string fillValue;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        [H5Name("dataType")]
        public string dataType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        [H5Name("datatype")]
        public string datatype;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        [H5Name("lower")]
        public string lower;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        [H5Name("upper")]
        public string upper;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        [H5Name("closure")]
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
            this.datatype = dataType;
            this.lower = lower;
            this.upper = upper;
            this.closure = closure;
        }
    }
}
