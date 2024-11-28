using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class BerthPosition : Layout, IS131Feature, IBerthPosition
    {
        public float AvailableBerthingLength { get; set; } = 0.0f;
        public string BollardDescription { get; set; } = string.Empty;
        public float BollardPull { get; set; } = 0.0f;
        public string[] BollardNumber { get; set; } = Array.Empty<string>();
        public string GLNExtension { get; set; } = string.Empty;
        public string[] MetreMarkNumber { get; set; } = Array.Empty<string>();
        public string[] ManifoldNumber { get; set; } = Array.Empty<string>();
        public string RampNumber { get; set; } = string.Empty;
        public string LocationByText { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new BerthPosition()
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
                Links = Links == null
                    ? Array.Empty<ILink>()
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink),
                AvailableBerthingLength = AvailableBerthingLength,
                BollardDescription = BollardDescription,
                BollardPull = BollardPull,
                BollardNumber = BollardNumber,
                GLNExtension = GLNExtension,
                MetreMarkNumber = MetreMarkNumber,
                ManifoldNumber = ManifoldNumber,
                RampNumber = RampNumber,
                LocationByText = LocationByText
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

            base.FromXml(node, mgr);

            //public float AvailableBerthingLength { get; set; } = 0.0f;
            var availableBerthingLengthNode = node.SelectSingleNode("availableBerthingLength", mgr);
            if (availableBerthingLengthNode != null && availableBerthingLengthNode.HasChildNodes)
            {
                if (float.TryParse(availableBerthingLengthNode.FirstChild?.InnerText, out float availableBerthingLengthValue))
                {
                    AvailableBerthingLength = availableBerthingLengthValue;
                }
            }

            //public string BollardDescription { get; set; } = string.Empty;
            var bollardDescriptionNode = node.SelectSingleNode("bollardDescription", mgr);
            if (bollardDescriptionNode != null && bollardDescriptionNode.HasChildNodes)
            {
                BollardDescription = bollardDescriptionNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public float BollardPull { get; set; } = 0.0f;
            var bollardPullNode = node.SelectSingleNode("bollardPull", mgr);
            if (bollardPullNode != null && bollardPullNode.HasChildNodes)
            {
                if (float.TryParse(bollardPullNode.FirstChild?.InnerText, out float bollardPullNodeValue))
                {
                    BollardPull = bollardPullNodeValue;
                }
            }

            //public string[] BollardNumber { get; set; } = Array.Empty<string>();
            var bollardNumberNodes = node.SelectNodes("bollardNumber", mgr);
            if (bollardNumberNodes != null && bollardNumberNodes.Count > 0)
            {
                var bollardNumbers = new List<string>();
                foreach (XmlNode bollardNumberNode in bollardNumberNodes)
                {
                    if (bollardNumberNode != null && bollardNumberNode.HasChildNodes && String.IsNullOrEmpty(bollardNumberNode.FirstChild?.InnerText) == false)
                    {
                        bollardNumbers.Add(bollardNumberNode.FirstChild?.InnerText);
                    }
                }

                bollardNumbers.Sort();
                BollardNumber = bollardNumbers.ToArray();
            }

            //public string GLNExtension { get; set; } = string.Empty;
            var gLNExtensionNode = node.SelectSingleNode("gLNExtension", mgr);
            if (gLNExtensionNode != null && gLNExtensionNode.HasChildNodes)
            {
                GLNExtension = gLNExtensionNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string[] MetreMarkNumber { get; set; } = Array.Empty<string>();
            var metreMarkNumberNodes = node.SelectNodes("metreMarkNumber", mgr);
            if (metreMarkNumberNodes != null && metreMarkNumberNodes.Count > 0)
            {
                var metreMarkNumbers = new List<string>();
                foreach (XmlNode metreMarkNumberNode in metreMarkNumberNodes)
                {
                    if (metreMarkNumberNode != null && metreMarkNumberNode.HasChildNodes && String.IsNullOrEmpty(metreMarkNumberNode.FirstChild?.InnerText) == false)
                    {
                        metreMarkNumbers.Add(metreMarkNumberNode.FirstChild?.InnerText);
                    }
                }

                metreMarkNumbers.Sort();
                MetreMarkNumber = metreMarkNumbers.ToArray();
            }

            //public string[] ManifoldNumber { get; set; } = Array.Empty<string>();
            var manifoldNumberNodes = node.SelectNodes("manifoldNumber", mgr);
            if (manifoldNumberNodes != null && manifoldNumberNodes.Count > 0)
            {
                var manifoldNumbers = new List<string>();
                foreach (XmlNode manifoldNumberNode in manifoldNumberNodes)
                {
                    if (manifoldNumberNode != null && manifoldNumberNode.HasChildNodes && String.IsNullOrEmpty(manifoldNumberNode.FirstChild?.InnerText) == false)
                    {
                        manifoldNumbers.Add(manifoldNumberNode.FirstChild?.InnerText);
                    }
                }

                manifoldNumbers.Sort();
                ManifoldNumber = manifoldNumbers.ToArray();
            }

            //public string RampNumber { get; set; } = string.Empty;
            var rampNumberNode = node.SelectSingleNode("rampNumber", mgr);
            if (rampNumberNode != null && rampNumberNode.HasChildNodes)
            {
                RampNumber = rampNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string LocationByText { get; set; } = string.Empty;
            var locationByTextNode = node.SelectSingleNode("locationByText", mgr);
            if (locationByTextNode != null && locationByTextNode.HasChildNodes)
            {
                LocationByText = locationByTextNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            if (BollardNumber.Length == 0)
            {
                return "PILPNT02";
            }

            return "BRTHNO01";
        }
    }
}
