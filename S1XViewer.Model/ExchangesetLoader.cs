﻿using S1XViewer.Base;
using S1XViewer.Base.Interfaces;
using S1XViewer.Model.Interfaces;
using S1XViewer.Types;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Model
{
    public class ExchangesetLoader : IExchangesetLoader
    {
        private readonly IInjectableXmlDocument _injectableXmlDocument;

        /// <summary>
        ///     Publicly accessible XmlDocument that contains the exchangeset
        /// </summary>
        public XmlDocument ExchangesetXml { get; set; } = null;

        public List<DatasetInfo> DatasetInfoItems { get; set; } = new List<DatasetInfo>();

        /// <summary>
        ///     For autofac initialization
        /// </summary>
        public ExchangesetLoader(IInjectableXmlDocument injectableXmlDocument)
        {
            _injectableXmlDocument = injectableXmlDocument;
        }

        /// <summary>
        ///     Loads the specified filename and returns an initialized XmlDocument
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>XmlDocument</returns>
        public virtual XmlDocument Load(string fileName)
        {
            ExchangesetXml = _injectableXmlDocument.Load(fileName);
            return ExchangesetXml;
        }

        /// <summary>
        ///     Parses the specified XmlDocument and retrieves the standard and filename the exchangeset refers to
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>(string, List<string>)</returns>
        public virtual (string, List<string>) Parse(XmlDocument xmlDocument)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("S100XC", "http://www.iho.int/s100/xc/5.2");
            nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");

            var productSpecificationIdentifierNode = xmlDocument.DocumentElement?.SelectSingleNode("S100XC:productSpecification/S100XC:productIdentifier", nsmgr);
            string productStandard = string.Empty;

            if (productSpecificationIdentifierNode == null) // if empty, try S100 V5.0 mode
            {
                nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                nsmgr.AddNamespace("S100XC", "http://www.iho.int/s100/xc/5.0");
                nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");

                productSpecificationIdentifierNode = xmlDocument.DocumentElement?.SelectSingleNode("S100XC:productSpecification/S100XC:productIdentifier", nsmgr);
                if (productSpecificationIdentifierNode == null) // if empty, try S100 V4.0 mode
                {
                    nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                    nsmgr.AddNamespace("S100XC", "http://www.iho.int/s100/xc");
                    nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");

                    productSpecificationIdentifierNode = xmlDocument.DocumentElement?.SelectSingleNode("S100XC:productSpecification/S100XC:productIdentifier", nsmgr);
                }
            }

            if (productSpecificationIdentifierNode != null)
            {
                productStandard = productSpecificationIdentifierNode.InnerText;
            }

            DatasetInfoItems = new List<DatasetInfo>();

            var datasetDiscoveryNodes = xmlDocument.DocumentElement?.SelectNodes("S100XC:datasetDiscoveryMetadata/S100XC:S100_DatasetDiscoveryMetadata", nsmgr);
            var productFileNames = new List<string>();
            if (datasetDiscoveryNodes != null && datasetDiscoveryNodes.Count > 0)
            {
                foreach (XmlNode node in datasetDiscoveryNodes)
                {
                    XmlNode metaDataNode;
                    if (node.FirstChild?.Name.Contains("DatasetDiscoveryMetadata") == true)
                    {
                        metaDataNode = node.FirstChild;
                    }
                    else
                    {
                        metaDataNode = node;
                    }

                    var fileName = string.Empty;
                    var fileNameNode = metaDataNode.SelectSingleNode("S100XC:fileName", nsmgr);
                    if (fileNameNode != null)
                    {
                        fileName = fileNameNode.InnerText.Replace("file:/", "").LastPart("/");
                    }

                    string productFileName;
                    var filePathNode = metaDataNode.SelectSingleNode("S100XC:filePath", nsmgr);
                    if (filePathNode != null) // V4 compliancy
                    {
                        productFileName = $@"{filePathNode.InnerText.Replace("/", @"\")}\{fileName}";
                    }
                    else // V5 compliancy assumes static folder called DATASET_FILES and a known ProducerCode
                    {
                        var producerCodeNode = metaDataNode.SelectSingleNode("S100XC:producerCode", nsmgr);
                        if (producerCodeNode != null)
                        {
                            productFileName = $@"{productStandard}\DATASET_FILES\{producerCodeNode.InnerText.PadRight(4, char.Parse("0"))}\{fileName}";
                        }
                        else
                        {
                            productFileName = fileName;
                        }
                    }
                    productFileNames.Add(productFileName);

                    var temporalExtentNode = metaDataNode.SelectSingleNode("S100XC:temporalExtent", nsmgr);
                    if (temporalExtentNode == null)
                    {
                        temporalExtentNode = metaDataNode.SelectSingleNode("temporalExtent");
                    }

                    string timeInstantBegin = string.Empty;
                    string timeInstantEnd = string.Empty;
                    if (temporalExtentNode != null)
                    {
                        var timeInstantBeginNode = temporalExtentNode.SelectSingleNode("S100XC:timeInstantBegin", nsmgr);
                        if (timeInstantBeginNode != null)
                        {
                            timeInstantBegin = timeInstantBeginNode.InnerText;
                        }

                        var timeInstantEndNode = temporalExtentNode.SelectSingleNode("S100XC:timeInstantEnd", nsmgr);
                        if (timeInstantEndNode != null)
                        {
                            timeInstantEnd = timeInstantEndNode.InnerText;
                        }
                    }

                    var datasetInfoItem = new DatasetInfo() { DateTimeEnd = timeInstantEnd, DateTimeStart = timeInstantBegin, FileName = productFileName };
                    DatasetInfoItems.Add(datasetInfoItem);
                }

                return (productStandard, productFileNames);
            }

            return (string.Empty, new List<string>());
        }
    }
}
