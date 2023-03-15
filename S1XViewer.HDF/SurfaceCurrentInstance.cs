using HDF5CSharp.DataTypes;

namespace S1XViewer.HDF
{
    public struct SurfaceCurrentInstance
    {
        [Hdf5EntryName("surfaceCurrentSpeed")]
        public float speed;

        [Hdf5EntryName("surfaceCurrentDirection")]
        public float direction;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="height"></param>
        /// <param name="trend"></param>
        public SurfaceCurrentInstance(float speed, float direction)
        {
            this.speed = speed;
            this.direction = direction;
        }
    }
}
