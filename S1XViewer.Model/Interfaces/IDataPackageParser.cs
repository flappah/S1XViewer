using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Model.Interfaces
{
    public interface IDataPackageParser 
    {
        IDataParser[] DataParsers { get; set; }
        /// <summary>
        ///     If standard is specified it takes preference above the standard found in the GML file
        /// </summary>
        string UseStandard { get; set; }

        Task<IDataParser> GetDataParserAsync(XmlDocument xmlDocument);
        IDataParser GetDataParser(XmlDocument xmlDocument);
        Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument);
        IS1xxDataPackage Parse(XmlDocument xmlDocument);
    }
}