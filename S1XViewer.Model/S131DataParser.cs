using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async override Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            if (xmlDocument is null || xmlDocument.DocumentElement == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }

            var dataPackage = new S127DataPackage
            {
                Type = S1xxTypes.S127,
                RawXmlData = xmlDocument
            };

            XmlNamespaceManager nsmgr = GetAllNamespaces(xmlDocument);

            string invertLonLatString = _optionsStorage.Retrieve("checkBoxInvertLonLat");
            if (!bool.TryParse(invertLonLatString, out bool invertLonLat))
            {
                invertLonLat = false;
            }
            _geometryBuilderFactory.InvertLonLat = invertLonLat;

            var defaultCRSString = GetSrsName(xmlDocument.DocumentElement);
            _geometryBuilderFactory.DefaultCRS = defaultCRSString;
            dataPackage.DefaultCRS = int.Parse(defaultCRSString);

            // retrieve boundingbox
            var boundingBoxNodes = xmlDocument.GetElementsByTagName("gml:boundedBy");
            if (boundingBoxNodes != null && boundingBoxNodes.Count > 0)
            {
                dataPackage.BoundingBox = _geometryBuilderFactory.Create(boundingBoxNodes[0], nsmgr);
            }

            // retrieve imembers
            var informationFeatures = await Task.Run(() =>
            {
                XmlNodeList imemberNodes = xmlDocument.GetElementsByTagName("imember");
                var localInfoFeaturesList = new List<IInformationFeature>();

                foreach (XmlNode imemberNode in imemberNodes)
                {
                    IFeature? feature = _featureFactory.FromXml(imemberNode, nsmgr)?.DeepClone();
                    if (feature is IInformationFeature informationFeature)
                    {
                        localInfoFeaturesList.Add(informationFeature);
                    }
                }
                return localInfoFeaturesList;
            });

            // retrieve members
            var geoFeatures = new List<IGeoFeature>();
            var metaFeatures = new List<IMetaFeature>();

            await Task.Run(() =>
            {
                XmlNodeList memberNodes = xmlDocument.GetElementsByTagName("member");
                short i = 0;
                foreach (XmlNode memberNode in memberNodes)
                {
                    var percentage = ((double)i++ / (double)memberNodes.Count) * 100.0;
                    Progress?.Invoke(percentage);

                    IFeature? feature = _featureFactory.FromXml(memberNode, nsmgr)?.DeepClone();
                    if (feature is IGeoFeature geoFeature && memberNode.HasChildNodes)
                    {
                        var geometryOfMemberNode = memberNode.FirstChild?.SelectSingleNode("geometry");
                        if (geometryOfMemberNode != null && geometryOfMemberNode.HasChildNodes)
                        {
                            geoFeature.Geometry = _geometryBuilderFactory.Create(geometryOfMemberNode.ChildNodes[0], nsmgr);
                        }

                        geoFeatures.Add(geoFeature);
                    }
                    else
                    {
                        if (feature is IMetaFeature metaFeature && memberNode.HasChildNodes)
                        {
                            var geometryOfMemberNode = memberNode.FirstChild?.SelectSingleNode("geometry");
                            if (geometryOfMemberNode != null && geometryOfMemberNode.HasChildNodes)
                            {
                                metaFeature.Geometry = _geometryBuilderFactory.Create(geometryOfMemberNode.ChildNodes[0], nsmgr);
                            }

                            metaFeatures.Add(metaFeature);
                        }
                    }
                }

                Progress?.Invoke(0);
            });

            // Populate links between features
            //Parallel.ForEach(informationFeatures, (informationFeature) =>
            foreach (var informationFeature in informationFeatures)
            {
                ResolveLinks(informationFeature.Links, informationFeatures, metaFeatures, geoFeatures);
            }

            //Parallel.ForEach(metaFeatures, (metaFeature) =>
            foreach (var metaFeature in metaFeatures)
            {
                ResolveLinks(metaFeature.Links, informationFeatures, metaFeatures, geoFeatures);
            }

            //Parallel.ForEach(geoFeatures, (geoFeature) =>
            foreach (var geoFeature in geoFeatures)
            {
                ResolveLinks(geoFeature.Links, informationFeatures, metaFeatures, geoFeatures);
            }

            dataPackage.GeoFeatures = geoFeatures.ToArray();
            dataPackage.MetaFeatures = metaFeatures.ToArray();
            dataPackage.InformationFeatures = informationFeatures.ToArray();
            return dataPackage;
        }

        public override Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime)
        {
            throw new NotImplementedException();
        }
    }
}
