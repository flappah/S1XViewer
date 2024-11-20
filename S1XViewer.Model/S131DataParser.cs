using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace S1XViewer.Model
{
    public class S131DataParser : DataParserBase, IS131DataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;

        private readonly IGeometryBuilderFactory _geometryBuilderFactory;
        private readonly IFeatureFactory _featureFactory;

        /// <summary>
        ///     For autofac initialization
        /// </summary>
        public S131DataParser(IGeometryBuilderFactory geometryBuilderFactory, IFeatureFactory featureFactory, IOptionsStorage optionsStorage)
        {
            _geometryBuilderFactory = geometryBuilderFactory;
            _featureFactory = featureFactory;
            _optionsStorage = optionsStorage;
        }

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
