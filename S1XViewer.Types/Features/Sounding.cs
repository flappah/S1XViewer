﻿using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Drawing;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Sounding : GeoFeatureBase, ISounding, IS102Feature
    {
        public double Value { get; set; }
        public string[] QualityOfVerticalMeasurement { get; set; } = new string[0]; 
        public DateTime ReportedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string[] TechniqueOfVerticalMeasurement { get; set; } = new string[0];
        public IVerticalUncertainty? VerticalUncertainty { get; set; }
        
        /// <summary>
        ///     Renders the featur for use in ArcGIS Runtime
        /// </summary>
        /// <param name="featureTable"></param>
        /// <returns></returns>
        public override (string type, Feature feature, Esri.ArcGISRuntime.UI.Graphic? graphic) Render(IFeatureRendererManager featureRendererManager, SpatialReference? horizontalCRS)
        {
            Field idField = new Field(FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(FieldType.Text, "FeatureName", "Name", 255);

            if (Geometry is Polygon polygon)
            {
                var color = Color.Black;
                foreach(ColorRampItem schemeItem in featureRendererManager.ColorRamp)
                {
                    if (schemeItem.Between(Value))
                    {
                        color = System.Drawing.Color.FromArgb(schemeItem.Color.A, schemeItem.Color.R, schemeItem.Color.G, schemeItem.Color.B);
                        break;
                    }
                }

                var lineSym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, color, 1);
                var sym = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color, lineSym);
                var simpleRenderer = new SimpleRenderer(sym);

                var key = $"FilledPolyFeatures_{color.R.ToString()}{color.G.ToString()}{color.B.ToString()}";
                var featureCollectionTable = featureRendererManager.Get(key);
                if (featureCollectionTable == null)
                {
                    featureCollectionTable = featureRendererManager.Create(key, new List<Field> { idField, nameField }, GeometryType.Polygon, horizontalCRS, false, simpleRenderer);
                }

                Feature polyFeature = featureCollectionTable.CreateFeature();
                polyFeature.SetAttributeValue(idField, Id);
                polyFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                polyFeature.Geometry = Geometry;

                return (key, polyFeature, null);
            }

            return base.Render(featureRendererManager, horizontalCRS);
        }

        /// <summary>
        ///     Deepclones the feature
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IFeature DeepClone()
        {
            return new Sounding
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null
                    ? new DateRange()
                    : FixedDateRange.DeepClone() as IDateRange,
                Id = Id ?? "",
                PeriodicDateRange = PeriodicDateRange == null
                    ? new DateRange[0]
                    : Array.ConvertAll(PeriodicDateRange, p => p.DeepClone() as IDateRange),
                SourceIndication = SourceIndication == null
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                TextContent = TextContent == null
                    ? new TextContent[0]
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                QualityOfVerticalMeasurement = QualityOfVerticalMeasurement == null
                    ? new string[0]
                    : Array.ConvertAll(QualityOfVerticalMeasurement, s => s),
                ReportedDate = ReportedDate,
                Status= Status,
                TechniqueOfVerticalMeasurement = TechniqueOfVerticalMeasurement == null
                    ? new string[0]
                    : Array.ConvertAll(TechniqueOfVerticalMeasurement, s => s),
                VerticalUncertainty = VerticalUncertainty == null 
                    ? new VerticalUncertainty()
                    : VerticalUncertainty.DeepClone() as IVerticalUncertainty,
                Value = Value,
                Links = Links == null
                    ? new Link[0]
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            if (node.HasChildNodes)
            {
                if (node.FirstChild?.Attributes?.Count > 0 &&
                    node.FirstChild?.Attributes.Contains("gml:id") == true)
                {
                    Id = node.FirstChild.Attributes["gml:id"].InnerText;
                }
            }

            var periodicDateRangeNodes = node.FirstChild.SelectNodes("periodicDateRange", mgr);
            if (periodicDateRangeNodes != null && periodicDateRangeNodes.Count > 0)
            {
                var dateRanges = new List<DateRange>();
                foreach (XmlNode periodicDateRangeNode in periodicDateRangeNodes)
                {
                    var newDateRange = new DateRange();
                    newDateRange.FromXml(periodicDateRangeNode, mgr);
                    dateRanges.Add(newDateRange);
                }
                PeriodicDateRange = dateRanges.ToArray();
            }

            var fixedDateRangeNode = node.FirstChild.SelectSingleNode("fixedDateRange", mgr);
            if (fixedDateRangeNode != null && fixedDateRangeNode.HasChildNodes)
            {
                FixedDateRange = new DateRange();
                FixedDateRange.FromXml(fixedDateRangeNode, mgr);
            }

            var featureNameNodes = node.FirstChild.SelectNodes("featureName", mgr);
            if (featureNameNodes != null && featureNameNodes.Count > 0)
            {
                var featureNames = new List<FeatureName>();
                foreach (XmlNode featureNameNode in featureNameNodes)
                {
                    var newFeatureName = new FeatureName();
                    newFeatureName.FromXml(featureNameNode, mgr);
                    featureNames.Add(newFeatureName);
                }
                FeatureName = featureNames.ToArray();
            }

            var sourceIndication = node.FirstChild.SelectSingleNode("sourceIndication", mgr);
            if (sourceIndication != null && sourceIndication.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndication, mgr);
            }

            var textContentNodes = node.FirstChild.SelectNodes("textContent", mgr);
            if (textContentNodes != null && textContentNodes.Count > 0)
            {
                var textContents = new List<TextContent>();
                foreach (XmlNode textContentNode in textContentNodes)
                {
                    if (textContentNode != null && textContentNode.HasChildNodes)
                    {
                        var content = new TextContent();
                        content.FromXml(textContentNode, mgr);
                        textContents.Add(content);
                    }
                }
                TextContent = textContents.ToArray();
            }


            // TODO: implement remaining XML data decoding!!!!!



            var linkNodes = node.FirstChild.SelectNodes("*[boolean(@xlink:href)]", mgr);
            if (linkNodes != null && linkNodes.Count > 0)
            {
                var links = new List<Link>();
                foreach (XmlNode linkNode in linkNodes)
                {
                    var newLink = new Link();
                    newLink.FromXml(linkNode, mgr);
                    links.Add(newLink);
                }
                Links = links.ToArray();
            }

            return this;
        }
    }
}
