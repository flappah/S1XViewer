using HDF5CSharp.DataTypes;

namespace S1XViewer.HDF
{
    public struct WaterLevelWithUncertaintyInstance
    {
        [Hdf5EntryName("waterLevelHeight")]
        public float waterLevelHeight;

        [Hdf5EntryName("waterLevelTrend")]
        public short waterLevelTrend;

        [Hdf5EntryName("uncertainty")]
        public float uncertainty;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="height"></param>
        /// <param name="trend"></param>
        /// <param name="unc"></param>
        public WaterLevelWithUncertaintyInstance(float height, short trend, float unc)
        {
            this.waterLevelHeight = height;
            this.waterLevelTrend = trend;
            this.uncertainty = unc;
        }
    }
}
