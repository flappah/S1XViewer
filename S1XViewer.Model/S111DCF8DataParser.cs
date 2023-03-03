using S1XViewer.Model.Interfaces;
using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace S1XViewer.Model
{
    public class S111DCF8DataParser : DataParserBase, IS111DCF8DataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;
        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        public override IS1xxDataPackage Parse(long hdf5FileId)
        {
            throw new NotImplementedException();
        }

        public override Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        public override Task<IS1xxDataPackage> ParseAsync(long hdf5FileId)
        {
            throw new NotImplementedException();
        }
    }
}
