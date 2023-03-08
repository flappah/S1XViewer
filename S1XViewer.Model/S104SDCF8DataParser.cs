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
    public class S104SDCF8DataParser : HdfDataParserBase, IS104SDCF8DataParser
    {
        public override event IDataParser.ProgressFunction? Progress;

        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            throw new NotImplementedException();
        }

        public override Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        public override Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime)
        {
            throw new NotImplementedException();
        }
    }
}
