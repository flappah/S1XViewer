using HDF5CSharp.DataTypes;
using System.Runtime.InteropServices;

namespace S1XViewer.HDF
{
    public struct SurfaceCurrentWithTimeInstance
    {
        [Hdf5EntryName("surfaceCurrentSpeed")]
        public float speed;

        [Hdf5EntryName("surfaceCurrentDirection")]
        public float direction;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 25)]
        [Hdf5EntryName("surfaceCurrentTime")]
        public string surfaceCurrentTime;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="height"></param>
        /// <param name="trend"></param>
        /// <param name="direction"></param>
        public SurfaceCurrentWithTimeInstance(float speed, float direction, string time)
        {
            this.speed = speed;
            this.direction = direction;
            this.surfaceCurrentTime = time;
        }
    }
}
