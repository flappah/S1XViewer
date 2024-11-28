using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types
{
    public abstract class GeoFeatureBase : FeatureBase, IGeoFeature
    {
        public IFeatureName[] FeatureName { get; set; } = Array.Empty<FeatureName>();
        public IDateRange FixedDateRange { get; set; } = new DateRange();
        public IDateRange[] PeriodicDateRange { get; set; } = Array.Empty<DateRange>();
        public ISourceIndication SourceIndication { get; set; } = new SourceIndication();
        public ITextContent[] TextContent { get; set; } = Array.Empty<TextContent>();

        /* S131 */
        public string LocationMRN { get; set; }  = string.Empty;
        public string GlobalLocationNumber { get; set; } = string.Empty;
        public IRxnCode[] RxnCode { get; set; } = Array.Empty<RxnCode>();
        public IGraphic[] Graphic { get; set; } = Array.Empty<Graphic>();
        public string Source {  get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public DateTime ReportedDate {  get; set; } = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        public Esri.ArcGISRuntime.Geometry.Geometry? Geometry { get; set; }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>

        public abstract string GetSymbolName();

        /// <summary>
        ///
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr);

            var featureNameNodes = node.SelectNodes("featureName", mgr);
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

            var fixedDateRangeNode = node.SelectSingleNode("fixedDateRange", mgr);
            if (fixedDateRangeNode != null && fixedDateRangeNode.HasChildNodes)
            {
                FixedDateRange = new DateRange();
                FixedDateRange.FromXml(fixedDateRangeNode, mgr);
            }

            var periodicDateRangeNodes = node.SelectNodes("periodicDateRange", mgr);
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

            var sourceIndicationNode = node.SelectSingleNode("sourceIndication", mgr);
            if (sourceIndicationNode != null && sourceIndicationNode.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndicationNode, mgr);
            }

            var textContentNodes = node.SelectNodes("textContent", mgr);
            if (textContentNodes != null && textContentNodes.Count > 0)
            {
                var textContentItems = new List<TextContent>();
                foreach (XmlNode textContentNode in textContentNodes)
                {
                    if (textContentNode != null && textContentNode.HasChildNodes)
                    {
                        var textContent = new ComplexTypes.TextContent();
                        textContent.FromXml(textContentNode, mgr);
                        textContentItems.Add(textContent);
                    }
                }
                TextContent = textContentItems.ToArray();
            }

            /* S131 */
            var locationMRNNode = node.SelectSingleNode("locationMRN", mgr);
            if (locationMRNNode != null && locationMRNNode.HasChildNodes)
            {
                LocationMRN = locationMRNNode.FirstChild?.InnerText ?? string.Empty;
            }

            var globalLocationNumberNode = node.SelectSingleNode("globalLocationNumber", mgr);
            if (globalLocationNumberNode != null && globalLocationNumberNode.HasChildNodes)
            {
                GlobalLocationNumber = globalLocationNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            var rxNCodeNodes = node.SelectNodes("rxNCode", mgr);
            if (rxNCodeNodes != null && rxNCodeNodes.Count > 0)
            {
                var rxnCodeItems = new List<RxnCode>();
                foreach (XmlNode rxnCodeNode in rxNCodeNodes)
                {
                    if (rxnCodeNode != null && rxnCodeNode.HasChildNodes)
                    {
                        var rxnCode = new ComplexTypes.RxnCode();
                        rxnCode.FromXml(rxnCodeNode, mgr);
                        rxnCodeItems.Add(rxnCode);
                    }
                }
                RxnCode = rxnCodeItems.ToArray();
            }

            var graphicNodes = node.SelectNodes("textContent", mgr);
            if (graphicNodes != null && graphicNodes.Count > 0)
            {
                var graphicItems = new List<Graphic>();
                foreach (XmlNode graphicNode in graphicNodes)
                {
                    if (graphicNode != null && graphicNode.HasChildNodes)
                    {
                        var graphic = new ComplexTypes.Graphic();
                        graphic.FromXml(graphicNode, mgr);
                        graphicItems.Add(graphic);
                    }
                }
                Graphic = graphicItems.ToArray();
            }

            var sourceNode = node.SelectSingleNode("source", mgr);
            if (sourceNode != null && sourceNode.HasChildNodes)
            {
                Source = sourceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var sourceTypeNode = node.SelectSingleNode("sourceType", mgr);
            if (sourceTypeNode != null && sourceTypeNode.HasChildNodes)
            {
                SourceType = sourceTypeNode.FirstChild?.InnerText ?? string.Empty;
            }

            var reportedDateNode = node.SelectSingleNode("reportedDate", mgr);
            if (reportedDateNode != null && reportedDateNode.HasChildNodes)
            {
                if (DateTime.TryParse(reportedDateNode.FirstChild?.InnerText, out DateTime reportedDateValue))
                {
                    ReportedDate = reportedDateValue;
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="distance"></param>
        /// <param name="bearing"></param>
        /// <returns></returns>
        protected (double Lat, double Lon) Destination((double Lat, double Lon) startPoint, double distance, double bearing)
        {
            var radius = 6378001;
            double lat1 = startPoint.Lat * (Math.PI / 180);
            double lon1 = startPoint.Lon * (Math.PI / 180);
            double brng = bearing * (Math.PI / 180);
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(distance / radius) + Math.Cos(lat1) * Math.Sin(distance / radius) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(distance / radius) * Math.Cos(lat1), Math.Cos(distance / radius) - Math.Sin(lat1) * Math.Sin(lat2));
            return (lat2 * (180 / Math.PI), lon2 * (180 / Math.PI));
        }

        /// <summary>
        ///     Renders an ARCGIS feature
        /// </summary>
        /// <param name="featureRendererManager"></param>
        /// <returns></returns>
        public virtual (string type, Esri.ArcGISRuntime.Data.Feature? feature, Esri.ArcGISRuntime.UI.Graphic? graphic) Render(IFeatureRendererManager featureRendererManager, SpatialReference? horizontalCRS)
        {
            if (featureRendererManager is null)
            {
                throw new ArgumentNullException(nameof(featureRendererManager));
            }

            Field idField = new Field(Esri.ArcGISRuntime.Data.FieldType.Text, "FeatureId", "Id", 50);
            Field nameField = new Field(Esri.ArcGISRuntime.Data.FieldType.Text, "FeatureName", "Name", 255);

            if (Geometry is MapPoint mapPoint)
            {
                FeatureCollectionTable? featureTable = featureRendererManager.Get("PointFeatures");
                if (featureTable != null)
                {
                    Esri.ArcGISRuntime.Data.Feature pointFeature = featureTable.CreateFeature();
                    pointFeature.SetAttributeValue(idField, Id);
                    pointFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                    pointFeature.Geometry = Geometry;

                    return ("PointFeatures", pointFeature, null);
                }
            }
            else if (Geometry is Polyline)
            {
                FeatureCollectionTable? featureTable = featureRendererManager.Get("LineFeatures");
                if (featureTable != null)
                {
                    Esri.ArcGISRuntime.Data.Feature lineFeature = featureTable.CreateFeature();
                    lineFeature.SetAttributeValue(idField, Id);
                    lineFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                    lineFeature.Geometry = Geometry;

                    return ("LineFeatures", lineFeature, null);
                }
            }
            else
            {
                FeatureCollectionTable? featureTable = featureRendererManager.Get("PolygonFeatures");
                if (featureTable != null)
                {
                    Esri.ArcGISRuntime.Data.Feature polyFeature = featureTable.CreateFeature();
                    polyFeature.SetAttributeValue(idField, Id);
                    polyFeature.SetAttributeValue(nameField, FeatureName?.First()?.Name);
                    polyFeature.Geometry = Geometry;

                    return ("PolygonFeatures", polyFeature, null);
                }
            }

            return ("NoFeatures", null, null);
        }
    }
}
