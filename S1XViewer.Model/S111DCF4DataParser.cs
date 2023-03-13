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

namespace S1XViewer.Model
{
    public class S111DCF4DataParser : HdfDataParserBase, IS111DCF4DataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;

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

            Hdf5Element hdf5S111Root;
            if (_cachedHdfTrees.ContainsKey(hdf5FileName))
            {
                hdf5S111Root = _cachedHdfTrees[hdf5FileName];
            }
            else
            {
                hdf5S111Root = await Task.Factory.StartNew((name) =>
                {
                    // load HDF file, spawned in a seperate task to keep UI responsive!
                    return HDF5CSharp.Hdf5.ReadTreeFileStructure(name.ToString());

                }, hdf5FileName).ConfigureAwait(false);

                _cachedHdfTrees.Add(hdf5FileName, hdf5S111Root);
            }

            // retrieve relevant time-frame from SurfaceCurrents collection
            foreach (HDF5CSharp.DataTypes.Hdf5Element? hdf5Element in hdf5S111Root.Children[1].Children)
            {
                if (hdf5Element != null)
                {
                    var dateTimeOfFirstRecord = hdf5Element.Attributes.Find("dateTimeOfFirstRecord");
                    var dateTimeOfLastRecord = hdf5Element.Attributes.Find("dateTimeOfLastRecord");

                    if (dateTimeOfFirstRecord != null && dateTimeOfLastRecord != null)
                    {

                    }
                }
            }


            // now retrieve positions 



            // retrieve directions and current speeds



            // build up featutes and wrap 'em in datapackage



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
