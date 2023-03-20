using S1XViewer.Model.Interfaces;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using S1XViewer.HDF;
using HDF5CSharp.DataTypes;
using S1XViewer.Model.Geometry;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Features;

namespace S1XViewer.Model
{
    public class S102DCF2DataParser : HdfDataParserBase, IS102DCF2DataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;

        private readonly IDatasetReader _datasetReader;
        private readonly IGeometryBuilderFactory _geometryBuilderFactory;

        /// <summary>
        ///     Empty constructor used for injection purposes
        /// </summary>
        /// <param name="datasetReader"></param>
        /// <param name="geometryBuilderFactory"></param>
        /// <param name="productSupport"></param>
        public S102DCF2DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory, IS111ProductSupport productSupport)
        {
            _datasetReader = datasetReader;
            _geometryBuilderFactory = geometryBuilderFactory;
            _productSupport = productSupport;
        }

        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            throw new NotImplementedException();
        }

        public override async Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public override async Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime)
        {
            if (string.IsNullOrEmpty(hdf5FileName))
            {
                throw new ArgumentException($"'{nameof(hdf5FileName)}' cannot be null or empty.", nameof(hdf5FileName));
            }

            if (selectedDateTime == null)
            {
                return new S1xxDataPackage
                {
                    Type = S1xxTypes.Null,
                    RawXmlData = null,
                    RawHdfData = null,
                    GeoFeatures = new IGeoFeature[0],
                    MetaFeatures = new IMetaFeature[0],
                    InformationFeatures = new IInformationFeature[0]
                };
            }

            var dataPackage = new S1xxDataPackage
            {
                Type = S1xxTypes.S111,
                RawXmlData = null,
                RawHdfData = null
            };

            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = await _productSupport.RetrieveHdf5FileAsync(hdf5FileName);
            long horizontalCRS = RetrieveHorizontalCRS(hdf5S111Root, hdf5FileName);

            // retrieve boundingbox
            var eastBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("eastBoundLongitude");
            var eastBoundLongitude = eastBoundLongitudeAttribute?.Value<double>(0f) ?? 0.0;
            var northBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("northBoundLatitude");
            var northBoundLatitude = northBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var southBoundLatitudeAttribute = hdf5S111Root.Attributes.Find("southBoundLatitude");
            var southBoundLatitude = southBoundLatitudeAttribute?.Value<double>(0f) ?? 0f;
            var westBoundLongitudeAttribute = hdf5S111Root.Attributes.Find("westBoundLongitude");
            var westBoundLongitude = westBoundLongitudeAttribute?.Value<double>(0f) ?? 0f;

            dataPackage.BoundingBox = _geometryBuilderFactory.Create("Envelope", new double[] { westBoundLongitude, eastBoundLongitude }, new double[] { southBoundLatitude, northBoundLatitude }, (int)horizontalCRS);

            Hdf5Element? featureElement = hdf5S111Root.Children.Find(elm => elm.Name.Equals("/BathymetryCoverage"));
            if (featureElement == null)
            {
                return dataPackage;
            }

            var selectedBathymetryCoverageElement = featureElement.Children[0];
            if (selectedBathymetryCoverageElement != null)
            {


            }

            Progress?.Invoke(100);
            return dataPackage;
        }
    }
}
