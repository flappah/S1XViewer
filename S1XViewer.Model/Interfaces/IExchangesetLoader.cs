using S1XViewer.Types;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Model.Interfaces
{
    public interface IExchangesetLoader
    {
        /// <summary>
        ///     Publicly accessible XmlDocument that contains the exchangeset
        /// </summary>
        XmlDocument ExchangesetXml { get; set; }

        List<DatasetInfo> DatasetInfoItems { get; set; }

        /// <summary>
        ///     Loads the specified filename and returns an initialized XmlDocument
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>XmlDocument</returns>
        XmlDocument Load(string fileName);

        /// <summary>
        ///     Parses the specified XmlDocument and retrieves the standard and filename the exchangeset refers to
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>(string, List<string>)</returns>
        (string, List<string>) Parse(XmlDocument xmlDocument);
    }
}