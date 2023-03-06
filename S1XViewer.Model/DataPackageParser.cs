using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Model
{
    public class DataPackageParser : IDataPackageParser
    {
        /// <summary>
        ///     Uses Autofac to insert all existing dataparsers in this property.
        /// </summary>
        public IDataParser[] DataParsers { get; set; }

        /// <summary>
        ///     If standard is specified it takes preference above the standard found in the GML file
        /// </summary>
        public string UseStandard { get; set; } = string.Empty;

        /// <summary>
        ///     Retrieves a datapackage parser based on the input from the XML file. Use of property is UseStandard is mandatory if an 
        ///     exchange file is used as its basic input
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public IDataParser GetDataParser(XmlDocument xmlDocument)
        {
            if (xmlDocument is null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }

            if (DataParsers == null || DataParsers.Count() == 0)
            {
                throw new Exception("No Dataparsers!");
            }

            string s12xTypeString = string.Empty;
            if (String.IsNullOrEmpty(UseStandard) == true)
            {
                string namespaceName = xmlDocument.DocumentElement?.Name ?? string.Empty;
                s12xTypeString = namespaceName.Substring(0, namespaceName.IndexOf(":"));
            }
            else
            {
                s12xTypeString = UseStandard;
            }

            S1xxTypes s12xType;
            try
            {
                s12xType = (S1xxTypes)Enum.Parse(typeof(S1xxTypes), s12xTypeString);
            }
            catch
            {
                // type can't be resolved just bail out
                return new NullDataParser();
            }

            var locatedDataParser =
                DataParsers.ToList().Find(tp => tp.GetType().Name.Contains(s12xType + "DataParser"));

            if (locatedDataParser != null)
            {
                return locatedDataParser;   
            }

            return new NullDataParser();
        }

        /// <summary>
        ///     Retrieves a datapackage parser based on the datacodingformat and the standard that is set in UseStandard
        /// </summary>
        /// <param name="dataCodingFormat"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IDataParser GetDataParser(byte dataCodingFormat)
        {
            if (DataParsers == null || DataParsers.Count() == 0)
            {
                throw new Exception("No Dataparsers!");
            }

            if (String.IsNullOrEmpty(UseStandard))
            {
                throw new Exception("No IHO standard specified!");
            }

            IDataParser locatedDataParser =
                DataParsers.ToList().Find(tp => tp.GetType().Name.Contains($"{UseStandard}DCF{dataCodingFormat}DataParser"));

            if (locatedDataParser != null)
            {
                return locatedDataParser;
            }

            return new NullDataParser();
        }

        /// <summary>
        ///     Parses XMLDocument, determines S1xx type and starts the corresponding dataparser to determine content
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>
        public async Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            var dataParser = GetDataParser(xmlDocument);
            return await dataParser.ParseAsync(xmlDocument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IS1xxDataPackage> ParseAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Parses XMLDocument, determines S1xx type and starts the corresponding dataparser to determine content
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>
        public IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            var dataParser = GetDataParser(xmlDocument);
            return dataParser.Parse(xmlDocument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IS1xxDataPackage Parse(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
