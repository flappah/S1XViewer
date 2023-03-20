using HDF5CSharp.DataTypes;
using S1XViewer.HDF.Interfaces;

namespace S1XViewer.HDF
{
    public abstract class ProductSupportBase : IProductSupportBase
    {
        protected static Dictionary<string, Hdf5Element> _cachedHdfTrees = new Dictionary<string, Hdf5Element>();

        /// <summary>
        ///     Async version of hdf5 file retrieval
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        public async Task<Hdf5Element> RetrieveHdf5FileAsync(string hdf5FileName)
        {
            Hdf5Element hdf5S111Root;
            if (_cachedHdfTrees.ContainsKey(hdf5FileName))
            {
                hdf5S111Root = _cachedHdfTrees[hdf5FileName];
            }
            else
            {
                hdf5S111Root = await Task.Factory.StartNew((name) =>
                {
                    // load HDF file, spawned in a seperate task to keep UI responsive!
                    return HDF5CSharp.Hdf5.ReadTreeFileStructure(name.ToString());

                }, hdf5FileName).ConfigureAwait(false);

                _cachedHdfTrees.Add(hdf5FileName, hdf5S111Root);
            }

            return hdf5S111Root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public abstract short GetTypeOfHorizontalCRS(string fileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        public abstract Task<(DateTime start, DateTime end)> RetrieveTimeFrameFromHdfDatasetAsync(string hdf5FileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureInstances"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        public abstract Hdf5Element? FindGroupByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureInstances"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        public abstract Hdf5Element? FindFeatureByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime);

        /// <summary>
        ///     Sync version of hdf5 file retrieval
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        public Hdf5Element RetrieveHdf5File(string hdf5FileName)
        {
            Hdf5Element hdf5S111Root;
            if (_cachedHdfTrees.ContainsKey(hdf5FileName))
            {
                return hdf5S111Root = _cachedHdfTrees[hdf5FileName];
            }
            else
            {
                return HDF5CSharp.Hdf5.ReadTreeFileStructure(hdf5FileName.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public abstract short GetDataCodingFormat(string fileName);

    }
}
