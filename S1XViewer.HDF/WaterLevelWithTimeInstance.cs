using HDF5CSharp.DataTypes;
using System.Runtime.InteropServices;

namespace S1XViewer.HDF
{
    public struct WaterLevelWithTimeInstance
    {
        [Hdf5EntryName("waterLevelHeight")]
        public float waterLevelHeight;

        [Hdf5EntryName("waterLevelTrend")]
        public short waterLevelTrend;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 25)]
        [Hdf5EntryName("waterLevelTime")]
        public string waterLevelTime;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="height"></param>
        /// <param name="trend"></param>
        public WaterLevelWithTimeInstance(float height, short trend, string time)
        {
            this.waterLevelHeight = height;
            this.waterLevelTrend = trend;
            this.waterLevelTime = time;
        }
    }
}
