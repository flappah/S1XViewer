﻿using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Model
{
    public class S128V11DataParser : DataParserBase, IS128V11DataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;

        private readonly IGeometryBuilderFactory _geometryBuilderFactory;
        private readonly IFeatureFactory _featureFactory;

        /// <summary>
        ///     For autofac initialization
        /// </summary>
        public S128V11DataParser(IGeometryBuilderFactory geometryBuilderFactory, IFeatureFactory featureFactory, IOptionsStorage optionsStorage)
        {
            _geometryBuilderFactory = geometryBuilderFactory;
            _featureFactory = featureFactory;
            _optionsStorage = optionsStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberNodes"></param>
        /// <param name="nsmgr"></param>
        /// <param name="invertLonLat"></param>
        /// <returns></returns>
        private (List<IGeoFeature> geoFeatures, List<IMetaFeature> metaFeatures, List<IInformationFeature> informationFeatures) RetrieveFeaturesFromXml(XmlNodeList? memberNodes, XmlNamespaceManager nsmgr, bool invertLonLat)
        {
            var geoFeatures = new List<IGeoFeature>();
            var metaFeatures = new List<IMetaFeature>();
            var informationFeatures = new List<IInformationFeature>();

            if (memberNodes != null)
            {
                _geometryBuilderFactory.InvertLonLat = invertLonLat;
                short i = 0;
                foreach (XmlNode memberNode in memberNodes)
                {
                    var percentage = ((double)i++ / (double)memberNodes.Count) * 100.0;
                    Progress?.Invoke(percentage);

                    IFeature? feature = _featureFactory.FromXml(memberNode, nsmgr, false)?.DeepClone();
                    if (feature != null)
                    {
                        if (feature is IGeoFeature geoFeature && memberNode.HasChildNodes)
                        {
                            var geometryOfMemberNode = memberNode.SelectSingleNode("S128:geometry", nsmgr);
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
                                var geometryOfMemberNode = memberNode.SelectSingleNode("S128:geometry", nsmgr);
                                if (geometryOfMemberNode != null && geometryOfMemberNode.HasChildNodes)
                                {
                                    metaFeature.Geometry = _geometryBuilderFactory.Create(geometryOfMemberNode.ChildNodes[0], nsmgr);
                                }

                                metaFeatures.Add(metaFeature);
                            }
                            else if (feature is IInformationFeature infoFeature && memberNode.HasChildNodes)
                            {
                                informationFeatures.Add(infoFeature);
                            }
                        }
                    }
                }
            }

            return (geoFeatures, metaFeatures, informationFeatures);
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

            var dataPackage = new S128DataPackage
            {
                Type = S1xxTypes.S128,
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
                       
            // retrieve members
            var geoFeatures = new List<IGeoFeature>();
            var metaFeatures = new List<IMetaFeature>();
            var informationFeatures = new List<IInformationFeature>();

            await Task.Run(() =>
            {
                XmlNodeList? memberNodes = xmlDocument.DocumentElement.ChildNodes;
                (geoFeatures, metaFeatures, informationFeatures) = RetrieveFeaturesFromXml(memberNodes, nsmgr, invertLonLat);

                if (invertLonLat == false && IsAnyPositionInverted(geoFeatures, metaFeatures))
                {
                    invertLonLat = true;
                    (geoFeatures, metaFeatures, informationFeatures) = RetrieveFeaturesFromXml(memberNodes, nsmgr, invertLonLat);
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

            dataPackage.InvertLonLat = invertLonLat;
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
            return new S128DataPackage
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
            return new S128DataPackage
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
