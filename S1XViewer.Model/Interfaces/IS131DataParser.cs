using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Model.Interfaces
{
    public interface IS131DataParser
    {
        event IDataParser.ProgressFunction? Progress;

        IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime);
        IS1xxDataPackage Parse(XmlDocument xmlDocument);
        Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime);
        Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument);
    }
}