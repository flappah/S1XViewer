using HDF5CSharp.DataTypes;

namespace S1XViewer.HDF
{
    public struct GeometryValueInstance
    {
        [Hdf5EntryName("longitude")]
        public double longitude;
        [Hdf5EntryName("latitude")]
        public double latitude;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public GeometryValueInstance(double longitude, double latitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;
        }
    }
}
