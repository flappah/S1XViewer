﻿using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Model
{
    public class S122DataParser : DataParserBase, IS122DataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;

        private readonly IGeometryBuilderFactory _geometryBuilderFactory;
        private readonly IFeatureFactory _featureFactory;

        /// <summary>
        ///     For autofac init
        /// </summary>
        /// <param name="geometryBuilderFactory"></param>
        /// <param name="featureFactory"></param>
        /// <param name="optionsStorage"></param>
        public S122DataParser(IGeometryBuilderFactory geometryBuilderFactory, IFeatureFactory featureFactory, IOptionsStorage optionsStorage)
        {
            _geometryBuilderFactory = geometryBuilderFactory;
            _featureFactory = featureFactory;
            _optionsStorage = optionsStorage;
        }                

        /// <summary>
        ///     Parses specified XMLDocument
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>
        public async override Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            if (xmlDocument is null || xmlDocument.DocumentElement == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }

            var dataPackage = new S122DataPackage
            {
                Type = S1xxTypes.S122,
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
            }).ConfigureAwait(false);

            // retrieve members
            var geoFeatures = new List<IGeoFeature>();
            var metaFeatures = new List<IMetaFeature>();

            await Task.Run(() =>
            {
                XmlNodeList memberNodes = xmlDocument.GetElementsByTagName("member");

                short i = 0;
                foreach (XmlNode memberNode in memberNodes)
                {
                    var percentage = ((double) i++ / (double) memberNodes.Count) * 100.0;
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
            }).ConfigureAwait(false);

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

        /// <summary>
        ///     Parses specified XMLDocument
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>
        public override IS1xxDataPackage Parse(XmlDocument xmlDocument)
        {
            return ParseAsync(xmlDocument).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     No implementation!
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        public override async Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime)
        {
            return new S122DataPackage
            {
                Type = S1xxTypes.Null,
                RawXmlData = null,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }

        /// <summary>
        ///      No implementation!
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        public override IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime)
        {
            return new S122DataPackage
            {
                Type = S1xxTypes.Null,
                RawXmlData = null,
                GeoFeatures = new IGeoFeature[0],
                MetaFeatures = new IMetaFeature[0],
                InformationFeatures = new IInformationFeature[0]
            };
        }
    }
}
