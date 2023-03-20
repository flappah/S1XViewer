using HDF5CSharp.DataTypes;
using S1XViewer.HDF.Interfaces;

namespace S1XViewer.HDF
{
    public class NullProductSupport : ProductSupportBase, INullProductSupport
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureInstances"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns>Hdf5Element</returns>
        public override Hdf5Element? FindFeatureByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime)
        {
            return new Hdf5Element("empty", Hdf5ElementType.Unknown, null, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureInstances"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns>Hdf5Element</returns>
        public override Hdf5Element? FindGroupByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime)
        {
            return new Hdf5Element("empty", Hdf5ElementType.Unknown, null, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public override short GetDataCodingFormat(string fileName)
        {
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override short GetTypeOfHorizontalCRS(string fileName)
        {
            return -1;
        }

        public override async Task<(DateTime start, DateTime end)> RetrieveTimeFrameFromHdfDatasetAsync(string hdf5FileName)
        {
            return (DateTime.Now, DateTime.Now);
        }
    }
}
