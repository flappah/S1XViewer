namespace S1XViewer.HDF.Interfaces
{
    public interface IProductSupportBase
    {
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
