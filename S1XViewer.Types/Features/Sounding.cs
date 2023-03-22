using Esri.ArcGISRuntime.Data;
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
        public string[] QualityOfVerticalMeasurement { get; set; }    
        public DateTime ReportedDate { get; set; }
        public string Status { get; set; }  
        public string[] TechniqueOfVerticalMeasurement { get; set; }
        public IVerticalUncertainty? VerticalUncertainty { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureTable"></param>
        /// <returns></returns>
        public override (Feature?, Esri.ArcGISRuntime.UI.Graphic?) Render(FeatureCollectionTable featureTable)
        {
            return (null, null);
        }

        /// <summary>
        ///     Creates an ARCGIS feature
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="featureTableCollection"></param>
        /// <param name="horizontalCRS"></param>
        /// <returns></returns>
        public virtual (Dictionary<System.Drawing.Color, FeatureCollectionTable>, Feature?) GetFeature(List<Field> fields, Dictionary<System.Drawing.Color, FeatureCollectionTable> featureTableCollection, SpatialReference? horizontalCRS)
        {
            if (fields is null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            if (featureTableCollection is null)
            {
                throw new ArgumentNullException(nameof(featureTableCollection));
            }

            Field idField = new Field(FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(FieldType.Text, "FeatureName", "Name", 255);

            if (Geometry is Polygon polygon)
            {
                System.Drawing.Color color = System.Drawing.Color.Black;
                if (Value <= -20.0)
                {
                    color = System.Drawing.Color.FromArgb(247, 148, 58);
                }
                else if (Value > -20.0 && Value <= -15.0)
                {
                    color = System.Drawing.Color.FromArgb(252, 179, 86);
                }
                else if (Value > -15.0 && Value <= -12.0)
                {
                    color = System.Drawing.Color.FromArgb(252, 188, 100);
                }
                else if (Value > -12.0 && Value <= -8.0)
                {
                    color = System.Drawing.Color.FromArgb(252, 198, 122);
                }
                else if (Value > -8.0 && Value <= -4.0)
                {
                    color = System.Drawing.Color.FromArgb(252, 203, 142);
                }
                else if (Value > -4.0 && Value <= -2.0)
                {
                    color = System.Drawing.Color.FromArgb(253, 244, 165);
                }
                else if (Value > -2.0 && Value <= -1.0)
                {
                    color = System.Drawing.Color.FromArgb(255, 247, 190);
                }
                else if (Value > -1.0 && Value <= 0.0)
                {
                    color = System.Drawing.Color.FromArgb(255, 249, 207);
                }
                else if (Value > 0.0 && Value <= 1.0)
                {
                    color = System.Drawing.Color.FromArgb(193, 239, 255);
                }
                else if (Value > 1.0 && Value <= 2.5)
                {
                    color = System.Drawing.Color.FromArgb(156, 232, 255);
                }
                else if (Value > 2.5 && Value <= 5.0)
                {
                    color = System.Drawing.Color.FromArgb(115, 223, 255);
                }
                else if (Value > 5.0 && Value <= 10.0)
                {
                    color = System.Drawing.Color.FromArgb(100, 215, 255);
                }
                else if (Value > 10.0 && Value <= 15.0)
                {
                    color = System.Drawing.Color.FromArgb(85, 210, 255);
                }
                else if (Value > 15.0 && Value <= 20.0)
                {
                    color = System.Drawing.Color.FromArgb(70, 206, 255);
                }
                else if (Value > 20.0 && Value <= 25.0)
                {
                    color = System.Drawing.Color.FromArgb(60, 202, 255);
                }
                else if (Value > 25.0 && Value <= 30.0)
                {
                    color = System.Drawing.Color.FromArgb(50, 202, 255);
                }
                else if (Value > 30.0 && Value <= 35.0)
                {
                    color = System.Drawing.Color.FromArgb(43, 202, 255);
                }
                else if (Value > 35.0 && Value <= 40.0)
                {
                    color = System.Drawing.Color.FromArgb(38, 202, 255);
                }
                else if (Value > 40.0 && Value <= 50.0)
                {
                    color = System.Drawing.Color.FromArgb(33, 198, 255);
                }
                else if (Value > 50.0 && Value <= 55.0)
                {
                    color = System.Drawing.Color.FromArgb(28, 187, 255);
                }
                else
                {
                    color = System.Drawing.Color.FromArgb(15, 176, 255);
                }

                lock (this)
                {
                    if (featureTableCollection.ContainsKey(color) == false)
                    {
                        var lineSym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, color, 1);
                        var sym = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color, lineSym);
                        var simpleRenderer = new SimpleRenderer(sym);

                        var filledPolysTable = new FeatureCollectionTable(fields, GeometryType.Polygon, horizontalCRS)
                        {
                            Renderer = simpleRenderer,
                            DisplayName = "Polygons"
                        };

                        featureTableCollection.Add(color, filledPolysTable);
                    }
                }

                Feature polyFeature = featureTableCollection[color].CreateFeature();
                polyFeature.SetAttributeValue(idField, Id);
                polyFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                polyFeature.Geometry = Geometry;
                return (featureTableCollection, polyFeature);
            }

            return (new Dictionary<System.Drawing.Color, FeatureCollectionTable>(), null);
        }

        /// <summary>
        /// 
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
