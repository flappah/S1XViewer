using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System.Runtime.CompilerServices;
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
        /// 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public async Task<IDataParser> GetDataParserAsync(XmlDocument xmlDocument)
        {
            if (xmlDocument is null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }

            if (DataParsers == null || DataParsers.Count() == 0)
            {
                throw new Exception("No Dataparsers!");
            }

            string namespaceName = xmlDocument.DocumentElement?.Name ?? string.Empty;
            string s12xTypeString = namespaceName.Substring(0, namespaceName.IndexOf(":"));
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

            IDataParser locatedDataParser =
                DataParsers.ToList().Find(tp => tp.GetType().Name.Contains(s12xType + "DataParser"));

            if (locatedDataParser != null)
            {
                return locatedDataParser;
            }

            return new NullDataParser();
        }

        /// <summary>
        /// 
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

            string namespaceName = xmlDocument.DocumentElement?.Name ?? string.Empty;
            string s12xTypeString = namespaceName.Substring(0, namespaceName.IndexOf(":"));
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
        ///     Parses XMLDocument, determines S1xx type and starts the corresponding dataparser to determine content
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>
        public async Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            var dataParser = await GetDataParserAsync(xmlDocument);
            return await dataParser.ParseAsync(xmlDocument);
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
    }
}
