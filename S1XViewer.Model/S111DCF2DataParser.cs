using HDF5CSharp.DataTypes;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace S1XViewer.Model
{
    public class S111DCF2DataParser : HdfDataParserBase, IS111DCF2DataParser
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
        public S111DCF2DataParser(IDatasetReader datasetReader, IGeometryBuilderFactory geometryBuilderFactory)
        {
            _datasetReader = datasetReader;
            _geometryBuilderFactory = geometryBuilderFactory;
        }

        /// <summary>
        ///     Parses specified HDF5 file
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime)
        {
            Progress?.Invoke(50);

            Hdf5Element hdf5S111Root = await RetrieveHdf5FileAsync(hdf5FileName);

            Hdf5Element? minGroup = FindGroupByDateTime(hdf5S111Root.Children[1].Children, selectedDateTime);
            if (minGroup != null)
            {
                //we've found the relevant group. Use this group to create features on by calculating its position
                var minGroupParentNode = (Hdf5Element)minGroup.Parent;
                var gridOriginLatitude = minGroupParentNode.Attributes.Find("gridOriginLatitude");
                var gridOriginLongitude = minGroupParentNode.Attributes.Find("gridOriginLongitude");
                var gridSpacingLatitudinal = minGroupParentNode.Attributes.Find("gridSpacingLatitudinal");
                var gridSpacingLongitudinal = minGroupParentNode.Attributes.Find("gridSpacingLongitudinal");

                var numPointsLatitudinalElement = minGroupParentNode.Attributes.Find("numPointsLatitudinal");
                int numPointsLatitude = numPointsLatitudinalElement?.Value<int>() ?? -1;

                var numPointsLongitudinalNode = minGroupParentNode.Attributes.Find("numPointsLongitudinal");
                int numPointsLongitude = numPointsLongitudinalNode?.Value<int>() ?? -1;

                if (numPointsLatitude != -1 && numPointsLongitude != -1)
                {
                    var surfaceCurrentInfos =
                          _datasetReader.ReadArrayOfFloats(hdf5FileName, minGroup.Children[0].Name, numPointsLatitude, numPointsLongitude);
                }
            }

            Progress?.Invoke(100);

            return new S1xxDataPackage
            {
                Type = S1xxTypes.Null,
                RawXmlData = null,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }

        /// <summary>
        ///     Parses specified HDF5 file
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {

            // load HDF file
            //HDF5CSharp.DataTypes.Hdf5Element tree = HDF5CSharp.Hdf5.ReadTreeFileStructure(fileName);

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

        /// <summary>
        ///     No implementation!
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
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

        /// <summary>
        ///     No implementation!
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
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
    }
}
