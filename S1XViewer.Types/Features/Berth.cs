using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Berth : Layout, IS131Feature, IBerth
    {
        public float AvailableBerthingLength { get; set; } = 0.0f;
        public string BollardDescription { get; set; } = string.Empty;
        public float BollardPull { get; set; } = 0.0f;
        public float MinimumBerthDepth { get; set; } = 0.0f;
        public float Elevation { get; set; } = 0.0f;
        public bool CathodicProtectionSystem { get; set; } = false;
        public string CategoryOfBerthLocation { get; set; } = string.Empty;
        public string PortFacilityNumber { get; set; } = string.Empty;
        public string[] BollardNumber { get; set; } = new string[0];
        public string GLNExtension { get; set; } = string.Empty;
        public string[] MetreMarkNumber { get; set; } = new string[0];
        public string[] ManifoldNumber { get; set; } = new string[0];
        public string RampNumber { get; set; } = string.Empty;
        public string LocationByText { get; set; } = string.Empty;
        public string MethodOfSecuring { get; set; } = string.Empty;
        public string UNLocationCode { get; set; } = string.Empty;
        public string TerminalIdentifier { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new Berth()
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
                BollardNumber = BollardNumber,
                BollardPull = BollardPull,
                MinimumBerthDepth = MinimumBerthDepth,
                Elevation = Elevation,
                CategoryOfBerthLocation = CategoryOfBerthLocation,
                CathodicProtectionSystem = CathodicProtectionSystem,
                PortFacilityNumber = PortFacilityNumber,
                GLNExtension = GLNExtension,
                MetreMarkNumber = MetreMarkNumber,
                ManifoldNumber = ManifoldNumber,
                RampNumber = RampNumber,
                LocationByText = LocationByText,
                MethodOfSecuring = MethodOfSecuring,
                UNLocationCode = UNLocationCode,
                TerminalIdentifier = TerminalIdentifier
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IFeature FromXml(System.Xml.XmlNode node, System.Xml.XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr, nameSpacePrefix);

            //public float AvailableBerthingLength { get; set; } = 0.0f;
            var availableBerthingLengthNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}availableBerthingLength", mgr);
            if (availableBerthingLengthNode != null && availableBerthingLengthNode.HasChildNodes)
            {
                if (float.TryParse(availableBerthingLengthNode.FirstChild?.InnerText, out float availableBerthingLengthValue))
                {
                    AvailableBerthingLength = availableBerthingLengthValue;
                }
            }

            //public string BollardDescription { get; set; } = string.Empty;
            var bollardDescriptionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}bollardDescription", mgr);
            if (bollardDescriptionNode != null && bollardDescriptionNode.HasChildNodes)
            {
                BollardDescription = bollardDescriptionNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public float BollardPull { get; set; } = 0.0f;
            var bollardPullNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}bollardPull", mgr);
            if (bollardPullNode != null && bollardPullNode.HasChildNodes)
            {
                if (float.TryParse(bollardPullNode.FirstChild?.InnerText, out float bollardPullNodeValue))
                {
                    BollardPull = bollardPullNodeValue;
                }
            }

            //public float MinimumBerthDepth { get; set; } = 0.0f;
            var minimumBerthDepthNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}minimumBerthDepth", mgr);
            if (minimumBerthDepthNode != null && minimumBerthDepthNode.HasChildNodes)
            {
                if (float.TryParse(minimumBerthDepthNode.FirstChild?.InnerText, out float minimumBerthDepthValue))
                {
                    MinimumBerthDepth = minimumBerthDepthValue;
                }
            }

            //public float Elevation { get; set; } = 0.0f;
            var elevationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}elevation", mgr);
            if (elevationNode != null && elevationNode.HasChildNodes)
            {
                if (float.TryParse(elevationNode.FirstChild?.InnerText, out float elevationNodeValue))
                {
                    Elevation = elevationNodeValue;
                }
            }

            //public bool CathodicProtectionSystem { get; set; } = false;
            var cathodicProtectionSystemNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}cathodicProtectionSystem", mgr);
            if (cathodicProtectionSystemNode != null && cathodicProtectionSystemNode.HasChildNodes)
            {
                if (bool.TryParse(cathodicProtectionSystemNode.FirstChild?.InnerText, out bool cathodicProtectionSystemValue))
                {
                    CathodicProtectionSystem = cathodicProtectionSystemValue;
                }
            }

            //public string CategoryOfBerthLocation { get; set; } = string.Empty;
            var categoryOfBerthLocationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfBerthLocation", mgr);
            if (categoryOfBerthLocationNode != null && categoryOfBerthLocationNode.HasChildNodes)
            {
                CategoryOfBerthLocation = categoryOfBerthLocationNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string PortFacilityNumber { get; set; } = string.Empty;
            var portFacilityNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}portFacilityNumber", mgr);
            if (portFacilityNumberNode != null && portFacilityNumberNode.HasChildNodes)
            {
                PortFacilityNumber = portFacilityNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string[] BollardNumber { get; set; } = new string[0];
            var bollardNumberNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}bollardNumber", mgr);
            if (bollardNumberNodes != null && bollardNumberNodes.Count > 0)
            {
                var bollardNumbers = new List<string>();
                foreach (XmlNode bollardNumberNode in bollardNumberNodes)
                {
                    if (bollardNumberNode != null && bollardNumberNode.HasChildNodes && String.IsNullOrEmpty(bollardNumberNode.FirstChild?.InnerText) == false)
                    {
                        bollardNumbers.Add(bollardNumberNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                bollardNumbers.Sort();
                BollardNumber = bollardNumbers.ToArray();
            }

            //public string GLNExtension { get; set; } = string.Empty;
            var gLNExtensionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}gLNExtension", mgr);
            if (gLNExtensionNode != null && gLNExtensionNode.HasChildNodes)
            {
                GLNExtension = gLNExtensionNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string[] MetreMarkNumber { get; set; } = new string[0];
            var metreMarkNumberNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}metreMarkNumber", mgr);
            if (metreMarkNumberNodes != null && metreMarkNumberNodes.Count > 0)
            {
                var metreMarkNumbers = new List<string>();
                foreach (XmlNode metreMarkNumberNode in metreMarkNumberNodes)
                {
                    if (metreMarkNumberNode != null && metreMarkNumberNode.HasChildNodes && String.IsNullOrEmpty(metreMarkNumberNode.FirstChild?.InnerText) == false)
                    {
                        metreMarkNumbers.Add(metreMarkNumberNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                metreMarkNumbers.Sort();
                MetreMarkNumber = metreMarkNumbers.ToArray();
            }

            //public string[] ManifoldNumber { get; set; } = new string[0];
            var manifoldNumberNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}manifoldNumber", mgr);
            if (manifoldNumberNodes != null && manifoldNumberNodes.Count > 0)
            {
                var manifoldNumbers = new List<string>();
                foreach (XmlNode manifoldNumberNode in manifoldNumberNodes)
                {
                    if (manifoldNumberNode != null && manifoldNumberNode.HasChildNodes && String.IsNullOrEmpty(manifoldNumberNode.FirstChild?.InnerText) == false)
                    {
                        manifoldNumbers.Add(manifoldNumberNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }

                manifoldNumbers.Sort();
                ManifoldNumber = manifoldNumbers.ToArray();
            }

            //public string RampNumber { get; set; } = string.Empty;
            var rampNumberNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}rampNumber", mgr);
            if (rampNumberNode != null && rampNumberNode.HasChildNodes)
            {
                RampNumber = rampNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string LocationByText { get; set; } = string.Empty;
            var locationByTextNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}locationByText", mgr);
            if (locationByTextNode != null && locationByTextNode.HasChildNodes)
            {
                LocationByText = locationByTextNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string MethodOfSecuring { get; set; } = string.Empty;
            var methodOfSecuringNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}methodOfSecuring", mgr);
            if (methodOfSecuringNode != null && methodOfSecuringNode.HasChildNodes)
            {
                MethodOfSecuring = methodOfSecuringNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string UNLocationCode { get; set; } = string.Empty;
            var uNLocationCodeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}uNLocationCode", mgr);
            if (uNLocationCodeNode != null && uNLocationCodeNode.HasChildNodes)
            {
                UNLocationCode = uNLocationCodeNode.FirstChild?.InnerText ?? string.Empty;
            }

            //public string TerminalIdentifier { get; set; } = string.Empty;
            var terminalIdentifierNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}terminalIdentifier", mgr);
            if (terminalIdentifierNode != null && terminalIdentifierNode.HasChildNodes)
            {
                TerminalIdentifier = terminalIdentifierNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            return "BRTHNO01";
        }
    }
}
