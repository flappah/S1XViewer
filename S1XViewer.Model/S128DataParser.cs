using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System.Collections.Generic;
using System.Xml;
using System.Threading.Tasks;

namespace S1XViewer.Model
{
    public class S128DataParser : DataParserBase, IS128DataParser
    {
        public delegate void ProgressFunction(double percentage);
        public override event IDataParser.ProgressFunction? Progress;

        private readonly IGeometryBuilderFactory _geometryBuilderFactory;
        private readonly IFeatureFactory _featureFactory;

        /// <summary>
        ///     For autofac initialization
        /// </summary>
        public S128DataParser(IGeometryBuilderFactory geometryBuilderFactory, IFeatureFactory featureFactory)
        {
            _geometryBuilderFactory = geometryBuilderFactory;
            _featureFactory = featureFactory;
        }

        /// <summary>
        ///     Parses specified XMLDocument
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>
        public async override Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument)
        {
            var dataPackage = new S1xxDataPackage
            {
                Type = S1xxTypes.S128,
                RawData = xmlDocument
            };

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("gml", "http://www.opengis.net/gml/3.2");
            nsmgr.AddNamespace("S128", "http://www.iho.int/S127/gml/1.0");
            nsmgr.AddNamespace("s100", "http://www.iho.int/s100gml/1.0");
            nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");

            // retrieve boundingbox
            var boundingBoxNodes = xmlDocument.GetElementsByTagName("gml:boundedBy");
            if (boundingBoxNodes != null && boundingBoxNodes.Count > 0)
            {
                dataPackage.BoundingBox = _geometryBuilderFactory.FromXml(boundingBoxNodes[0], nsmgr);
            }

            // retrieve imembers
            var informationFeatures = await Task.Run(() =>
            {
                XmlNodeList imemberNodes = xmlDocument.GetElementsByTagName("imember");
                var localInfoFeaturesList = new List<IInformationFeature>();

                foreach (XmlNode imemberNode in imemberNodes)
                {
                    var feature = _featureFactory.FromXml(imemberNode, nsmgr).DeepClone();
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
                    
                    var feature = _featureFactory.FromXml(memberNode, nsmgr).DeepClone();

                    if (feature is IGeoFeature geoFeature && memberNode.HasChildNodes)
                    {
                        var geometryOfMemberNode = memberNode.FirstChild?.SelectSingleNode("geometry");
                        if (geometryOfMemberNode != null && geometryOfMemberNode.HasChildNodes)
                        {
                            geoFeature.Geometry = _geometryBuilderFactory.FromXml(geometryOfMemberNode.ChildNodes[0], nsmgr);
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
                                metaFeature.Geometry = _geometryBuilderFactory.FromXml(geometryOfMemberNode.ChildNodes[0], nsmgr);
                            }

                            metaFeatures.Add(metaFeature);
                        }
                    }
                }

                Progress?.Invoke(0);
            });

            // Populate links between features
            Parallel.ForEach(informationFeatures, (informationFeature) =>
            {
                ResolveLinks(informationFeature.Links, informationFeatures, metaFeatures, geoFeatures);
            });

            Parallel.ForEach(metaFeatures, (metaFeature) =>
            {
                ResolveLinks(metaFeature.Links, informationFeatures, metaFeatures, geoFeatures);
            });

            Parallel.ForEach(geoFeatures, (geoFeature) =>
            {
                ResolveLinks(geoFeature.Links, informationFeatures, metaFeatures, geoFeatures);
            });

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
            var dataPackage = new S1xxDataPackage
            {
                Type = S1xxTypes.S128,
                RawData = xmlDocument
            };

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("gml", "http://www.opengis.net/gml/3.2");
            nsmgr.AddNamespace("S128", "http://www.iho.int/S127/gml/1.0");
            nsmgr.AddNamespace("s100", "http://www.iho.int/s100gml/1.0");
            nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");

            // retrieve boundingbox
            var boundingBoxNodes = xmlDocument.GetElementsByTagName("gml:boundedBy");
            if (boundingBoxNodes != null && boundingBoxNodes.Count > 0)
            {
                dataPackage.BoundingBox = _geometryBuilderFactory.FromXml(boundingBoxNodes[0], nsmgr);
            }

            // retrieve imembers
            XmlNodeList imemberNodes = xmlDocument.GetElementsByTagName("imember");
            var informationFeatures = new List<IInformationFeature>();

            foreach (XmlNode imemberNode in imemberNodes)
            {
                var feature = _featureFactory.FromXml(imemberNode, nsmgr).DeepClone();
                if (feature is IInformationFeature informationFeature)
                {
                    informationFeatures.Add(informationFeature);
                }
            }

            // retrieve members
            var geoFeatures = new List<IGeoFeature>();
            var metaFeatures = new List<IMetaFeature>();
            XmlNodeList memberNodes = xmlDocument.GetElementsByTagName("member");
            foreach (XmlNode memberNode in memberNodes)
            {
                var feature = _featureFactory.FromXml(memberNode, nsmgr).DeepClone();

                if (feature is IGeoFeature geoFeature && memberNode.HasChildNodes)
                {
                    var geometryOfMemberNode = memberNode.FirstChild?.SelectSingleNode("geometry");
                    if (geometryOfMemberNode != null && geometryOfMemberNode.HasChildNodes)
                    {
                        geoFeature.Geometry = _geometryBuilderFactory.FromXml(geometryOfMemberNode.ChildNodes[0], nsmgr);
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
                            metaFeature.Geometry = _geometryBuilderFactory.FromXml(geometryOfMemberNode.ChildNodes[0], nsmgr);
                        }

                        metaFeatures.Add(metaFeature);
                    }
                }
            }

            // Populate links between features
            foreach (IFeature infoFeature in informationFeatures)
            {
                ResolveLinks(infoFeature.Links, informationFeatures, metaFeatures, geoFeatures);
            }

            foreach (IFeature metaFeature in metaFeatures)
            {
                ResolveLinks(metaFeature.Links, informationFeatures, metaFeatures, geoFeatures);
            }

            foreach (IFeature geoFeature in geoFeatures)
            {
                ResolveLinks(geoFeature.Links, informationFeatures, metaFeatures, geoFeatures);
            }

            dataPackage.GeoFeatures = geoFeatures.ToArray();
            dataPackage.MetaFeatures = metaFeatures.ToArray();
            dataPackage.InformationFeatures = informationFeatures.ToArray();
            return dataPackage;
        }

        public override Task<IS1xxDataPackage> ParseAsync(long hdf5FileId)
        {
            throw new System.NotImplementedException();
        }

        public override IS1xxDataPackage Parse(long hdf5FileId)
        {
            throw new System.NotImplementedException();
        }
    }
}
