using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Model.Interfaces
{
    public interface IDataParser
    {
        delegate void ProgressFunction(double percentage);
        event ProgressFunction? Progress;

        /// <summary>
        ///     Parses specified XMLDocument. Async version
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>
        Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument);

        /// <summary>
        ///     Parses specified HDF5 file. Async version
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime);

        /// <summary>
        /// Parses specified XMLDocument. 
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>

        IS1xxDataPackage Parse(XmlDocument xmlDocument);

        /// <summary>
        ///     Parses specified HDF5 file
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime);

    }
}
