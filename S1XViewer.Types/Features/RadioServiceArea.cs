using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class RadioServiceArea : GeoFeatureBase, IRadioServiceArea, IS123Feature
    {
        public string CallSign { get; set; } = string.Empty;
        public string CategoryOfBroadcastCommunication { get; set; } = string.Empty;
        public string LanguageInformation { get; set; } = string.Empty;
        public IRadioCommunications[] RadioCommunications { get; set; } = new RadioCommunications[0];
        public string Status { get; set; } = string.Empty;
        public string TimeReference { get; set; } = string.Empty;
        public string TransmissionPower { get; set; } = string.Empty;
        public string TxIdentChar { get; set; } = string.Empty;
        public string TxTrafficList { get; set; } = string.Empty;

        /// <summary>
        ///     Deep clones the object
        /// </summary>
        /// <returns>IComplexType</returns>
        public override IFeature DeepClone()
        {
            return new RadioServiceArea
            {
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                FixedDateRange = FixedDateRange == null
                    ? new DateRange()
                    : FixedDateRange.DeepClone() as IDateRange,
                Id = Id,
                PeriodicDateRange = PeriodicDateRange == null
                    ? Array.Empty<DateRange>()
                    : Array.ConvertAll(PeriodicDateRange, p => p.DeepClone() as IDateRange),
                SourceIndication = SourceIndication == null
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                TextContent = TextContent == null
                    ? Array.Empty<TextContent>()
                    : Array.ConvertAll(TextContent, t => t.DeepClone() as ITextContent),
                Geometry = Geometry,
                CallSign = CallSign,
                CategoryOfBroadcastCommunication = CategoryOfBroadcastCommunication,
                LanguageInformation = LanguageInformation,
                RadioCommunications = RadioCommunications == null
                    ? Array.Empty<RadioCommunications>()
                    : Array.ConvertAll(RadioCommunications, r => r.DeepClone() as IRadioCommunications),
                Status = Status,
                TimeReference = TimeReference,
                TransmissionPower = TransmissionPower,
                TxIdentChar = TxIdentChar,
                TxTrafficList = TxTrafficList,
                Links = Links == null
                    ? Array.Empty<Link>()
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
            };
        }

        /// <summary>
        ///     Reads the data from an XML dom
        /// </summary>
        /// <param name="node">current node to use as a starting point for reading</param>
        /// <param name="mgr">xml namespace manager</param>
        /// <returns>IFeature</returns>
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr);

            var callSignNode = node.SelectSingleNode("callSign", mgr);
            if (callSignNode != null && callSignNode.HasChildNodes)
            {
                CallSign = callSignNode.FirstChild.InnerText;
            }

            var categoryOfBroadcastCommunicationNode = node.SelectSingleNode("categoryOfBroadcastCommunication", mgr);
            if (categoryOfBroadcastCommunicationNode != null && categoryOfBroadcastCommunicationNode.HasChildNodes)
            {
                CategoryOfBroadcastCommunication = categoryOfBroadcastCommunicationNode.FirstChild?.InnerText ?? string.Empty;
            }

            var languageInformationNode = node.SelectSingleNode("languageInformation", mgr);
            if (languageInformationNode != null && languageInformationNode.HasChildNodes)
            {
                LanguageInformation = languageInformationNode.FirstChild?.InnerText ?? string.Empty;
            }

            var radioCommunicationNodes = node.SelectNodes("radiocommunications", mgr);
            if (radioCommunicationNodes != null && radioCommunicationNodes.Count > 0)
            {
                var radioCommunications = new List<RadioCommunications>();
                foreach (XmlNode radioCommunicationNode in radioCommunicationNodes)
                {
                    if (radioCommunicationNode != null && radioCommunicationNode.HasChildNodes)
                    {
                        var newRadioCommunications = new RadioCommunications();
                        newRadioCommunications.FromXml(radioCommunicationNode, mgr);
                        radioCommunications.Add(newRadioCommunications);
                    }
                }
                RadioCommunications = radioCommunications.ToArray();
            }

            var statusNode = node.SelectSingleNode("status", mgr);
            if (statusNode != null && statusNode.HasChildNodes)
            {
                Status = statusNode.FirstChild?.InnerText ?? string.Empty;
            }

            var timeReferenceNode = node.SelectSingleNode("timeReference", mgr);
            if (timeReferenceNode != null && timeReferenceNode.HasChildNodes)
            {
                TimeReference = timeReferenceNode.FirstChild?.InnerText ?? string.Empty;
            }

            var transmissionPowerNode = node.SelectSingleNode("transmissionPower", mgr);
            if (transmissionPowerNode != null && transmissionPowerNode.HasChildNodes)
            {
                TransmissionPower = transmissionPowerNode.FirstChild?.InnerText ?? string.Empty;
            }

            var txIdentCharNode = node.SelectSingleNode("txIdentChar", mgr);
            if (txIdentCharNode != null && txIdentCharNode.HasChildNodes)
            {
                TxIdentChar = txIdentCharNode.FirstChild?.InnerText ?? string.Empty;
            }

            var txTrafficListNode = node.SelectSingleNode("txTrafficList", mgr);
            if (txTrafficListNode != null && txTrafficListNode.HasChildNodes)
            {
                TxTrafficList = txTrafficListNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            return "";
        }
    }
}
