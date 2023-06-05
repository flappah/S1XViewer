using HDF5CSharp.DataTypes;

namespace S1XViewer.HDF
{
    public struct WaterLevelInstance
    {
        [Hdf5EntryName("waterLevelHeight")]
        public float height;

        [Hdf5EntryName("waterLevelTrend")]
        public short trend;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="height"></param>
        /// <param name="trend"></param>
        public WaterLevelInstance(float height, short trend)
        {
            this.height = height;
            this.trend = trend;
        }
    }
}
