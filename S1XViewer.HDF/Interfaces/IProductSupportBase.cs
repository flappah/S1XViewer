using HDF5CSharp.DataTypes;

namespace S1XViewer.HDF.Interfaces
{
    public interface IProductSupportBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        Task<Hdf5Element> RetrieveHdf5FileAsync(string hdf5FileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        Hdf5Element RetrieveHdf5File(string hdf5FileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        Task<(DateTime start, DateTime end)> RetrieveTimeFrameFromHdfDatasetAsync(string hdf5FileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureInstances"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        Hdf5Element? FindGroupByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureInstances"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        Hdf5Element? FindFeatureByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        short GetDataCodingFormat(string fileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        short GetTypeOfHorizontalCRS(string fileName);


    }
}
