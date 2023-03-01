using S1XViewer.Base.Interfaces;
using System.Xml;

namespace S1XViewer.Model
{
    public class DataLoader
    {
        private readonly IInjectableXmlDocument _injectableXmlDocument;

        /// <summary>
        ///     For autofac initialization
        /// </summary>
        public DataLoader(IInjectableXmlDocument injectableXmlDocument)
        {
            _injectableXmlDocument = injectableXmlDocument;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual XmlDocument Load(string fileName)
        {
            var xmlDoc = _injectableXmlDocument.Load(fileName);
            return xmlDoc;
        }
    }
}
