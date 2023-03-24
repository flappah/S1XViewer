using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class TidalStreamFloodEbb : GeoFeatureBase, ITidalStreamFloodEbb, IS104Feature, IVectorFeature
    {
        public string[] CategoryOfTidalStream { get; set; } = new string[0];
        public Orientation Orientation { get; set; }
        public Speed Speed { get; set; }

        /// <summary>
        ///     Renders an ARCGIS feature
        /// </summary>
        /// <param name="featureRendererManager"></param>
        /// <returns></returns>
        public override (string type, Feature feature, Esri.ArcGISRuntime.UI.Graphic? graphic) Render(IFeatureRendererManager featureRendererManager, SpatialReference? horizontalCRS)
        {
            Field idField = new Field(FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(FieldType.Text, "FeatureName", "Name", 255);

            if (Geometry is MapPoint mapPoint)
            {
                if (mapPoint != null)
                {
                    (double Lat, double Lon) secondPoint = (mapPoint.Y, mapPoint.X);
                    double width = 0.75;
                    System.Drawing.Color color = System.Drawing.Color.Black;

                    switch (Speed.SpeedMaximum)
                    {
                        case <= 0.5:
                            secondPoint = Destination((mapPoint.Y, mapPoint.X), 150, Orientation.OrientationValue);
                            width = 1.5;
                            color = System.Drawing.Color.FromArgb(118, 82, 226);
                            break;

                        case > 0.5 and <= 1.0:
                            secondPoint = Destination((mapPoint.Y, mapPoint.X), 250, Orientation.OrientationValue);
                            width = 2.5;
                            color = System.Drawing.Color.FromArgb(72, 152, 211);
                            break;

                        case > 1.0 and <= 2.0:
                            secondPoint = Destination((mapPoint.Y, mapPoint.X), 300, Orientation.OrientationValue);
                            width = 2.5;
                            color = System.Drawing.Color.FromArgb(97, 203, 229);
                            break;

                        case > 2.0 and <= 3.0:
                            secondPoint = Destination((mapPoint.Y, mapPoint.X), 400, Orientation.OrientationValue);
                            width = 3;
                            color = System.Drawing.Color.FromArgb(109, 188, 69);
                            break;

                        case > 3.0 and <= 5.0:
                            secondPoint = Destination((mapPoint.Y, mapPoint.X), 500, Orientation.OrientationValue);
                            width = 3;
                            color = System.Drawing.Color.FromArgb(180, 220, 0);
                            break;

                        case > 5.0 and <= 7.0:
                            secondPoint = Destination((mapPoint.Y, mapPoint.X), 500, Orientation.OrientationValue);
                            width = 4;
                            color = System.Drawing.Color.FromArgb(205, 193, 0);
                            break;

                        case > 7.0 and <= 10.0:
                            secondPoint = Destination((mapPoint.Y, mapPoint.X), 500, Orientation.OrientationValue);
                            width = 4;
                            color = System.Drawing.Color.FromArgb(248, 167, 24);
                            break;

                        case > 10.0 and <= 13.0:
                            secondPoint = Destination((mapPoint.Y, mapPoint.X), 500, Orientation.OrientationValue);
                            width = 4;
                            color = System.Drawing.Color.FromArgb(247, 162, 157);
                            break;

                        case > 13.0:
                            secondPoint = Destination((mapPoint.Y, mapPoint.X), 500, Orientation.OrientationValue);
                            width = 5;
                            color = System.Drawing.Color.FromArgb(255, 30, 30);
                            break;
                    }

                    var lineGeometry = new Polyline(new List<MapPoint> { mapPoint, new MapPoint(secondPoint.Lon, secondPoint.Lat) });

                    var symbol = new SimpleLineSymbol();
                    symbol.MarkerStyle = SimpleLineSymbolMarkerStyle.Arrow;
                    symbol.Color = color;
                    symbol.Width = width;

                    var graphic = new Esri.ArcGISRuntime.UI.Graphic();
                    graphic.Geometry = lineGeometry;
                    graphic.Symbol = symbol;

                    FeatureCollectionTable featureTable = featureRendererManager.Get("VectorFeatures");
                    Feature vectorFeature = featureTable.CreateFeature();
                    vectorFeature.SetAttributeValue(idField, Id);
                    vectorFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                    vectorFeature.Geometry = Geometry;

                    return ("VectorFeatures", vectorFeature, graphic);
                }
            }

            return base.Render(featureRendererManager, horizontalCRS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IFeature DeepClone()
        {
            return new TidalStreamFloodEbb
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null
                    ? new DateRange()
                    : FixedDateRange.DeepClone() as IDateRange,
                Id = Id,
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
                CategoryOfTidalStream = CategoryOfTidalStream == null
                    ? new string[0]
                    : Array.ConvertAll(CategoryOfTidalStream, s => s),
                Orientation = Orientation == null ? null : Orientation.DeepClone() as Orientation,
                Speed = Speed == null ? null : Speed.DeepClone() as Speed,
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
        /// <exception cref="NotImplementedException"></exception>
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

            var periodicDateRangeNodes = node.FirstChild?.SelectNodes("periodicDateRange", mgr);
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

            var categoryOfTidalStreamNodes = node.FirstChild.SelectNodes("categoryOfTidalStream", mgr);
            if (categoryOfTidalStreamNodes != null && categoryOfTidalStreamNodes.Count > 0)
            {
                var tidalStreamCategories = new List<string>();
                foreach (XmlNode categoryOfTidalStreamNode in categoryOfTidalStreamNodes)
                {
                    if (categoryOfTidalStreamNode != null && categoryOfTidalStreamNode.HasChildNodes)
                    {
                        tidalStreamCategories.Add(categoryOfTidalStreamNode.FirstChild.InnerText);
                    }
                }
                CategoryOfTidalStream = tidalStreamCategories.ToArray();
            }

            var orientationNode = node.FirstChild.SelectSingleNode("orientation", mgr);
            if (orientationNode != null && orientationNode.HasChildNodes)
            {
                Orientation = new Orientation();
                Orientation.FromXml(orientationNode, mgr);
            }

            var speedNode = node.FirstChild.SelectSingleNode("speed", mgr);
            if (speedNode != null && speedNode.HasChildNodes)
            {
                Speed = new Speed();
                Speed.FromXml(speedNode, mgr);
            }

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
