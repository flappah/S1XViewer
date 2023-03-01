using S1XViewer.Model.Interface;
using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System.Threading.Tasks;
using System.Xml;

namespace S1XViewer.Model
{
    public class NullDataParser : DataParserBase, INullDataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            return new S1xxDataPackage
            {
                Type = S1xxTypes.Null,
                RawData = xmlDocument,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature [0],
                InformationFeatures = new IInformationFeature[0]
            };
        }

        public override IS1xxDataPackage Parse(long hdf5FileId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public async override Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            return new S1xxDataPackage
            {
                Type = S1xxTypes.Null,
                RawData = xmlDocument,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }

        public override Task<IS1xxDataPackage> ParseAsync(long hdf5FileId)
        {
            throw new System.NotImplementedException();
        }
    }
}
