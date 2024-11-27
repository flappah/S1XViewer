using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class Terminal : Layout, IS131Feature, ITerminal
    {
        public string PortFacilityNumber { get; set; } = string.Empty;
        public string CategoryOfHarbourFacility { get; set; } = string.Empty;
        public string[] CategoryOfCargo { get; set; } = new string[0];
        public string[] Product { get; set; } = new string[0];
        public string TerminalIdentifier { get; set; } = string.Empty;
        public string SMDGTerminalCode { get; set; } = string.Empty;
        public string UNLocationCode { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new Terminal()
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
                    ? Array.Empty<Link>()
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink),
                PortFacilityNumber = PortFacilityNumber,
                CategoryOfHarbourFacility = CategoryOfHarbourFacility,
                CategoryOfCargo = CategoryOfCargo == null ? new string[0] : Array.ConvertAll(CategoryOfCargo, s => s),
                Product = Product == null ? new string[0] : Array.ConvertAll(Product, s => s),
                TerminalIdentifier = TerminalIdentifier,
                SMDGTerminalCode = SMDGTerminalCode,
                UNLocationCode = UNLocationCode
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

            var portFacilityNumberNode = node.SelectSingleNode("portFacilityNumber", mgr);
            if (portFacilityNumberNode != null && portFacilityNumberNode.HasChildNodes)
            {
                PortFacilityNumber = portFacilityNumberNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfHarbourFacilityNode = node.SelectSingleNode("categoryOfHarbourFacility", mgr);
            if (categoryOfHarbourFacilityNode != null && categoryOfHarbourFacilityNode.HasChildNodes)
            {
                CategoryOfHarbourFacility = categoryOfHarbourFacilityNode.FirstChild?.InnerText ?? string.Empty;
            }

            var categoryOfCargoNodes = node.SelectNodes("categoryOfCargo", mgr);
            if (categoryOfCargoNodes != null && categoryOfCargoNodes.Count > 0)
            {
                var categoryOfCargos = new List<string>();
                foreach (XmlNode categoryOfCargoNode in categoryOfCargoNodes)
                {
                    if (categoryOfCargoNode != null && categoryOfCargoNode.HasChildNodes && String.IsNullOrEmpty(categoryOfCargoNode.FirstChild?.InnerText) == false)
                    {
                        categoryOfCargos.Add(categoryOfCargoNode.FirstChild?.InnerText);
                    }
                }

                categoryOfCargos.Sort();
                CategoryOfCargo = categoryOfCargos.ToArray();
            }

            var productNodes = node.SelectNodes("product", mgr);
            if (productNodes != null && productNodes.Count > 0)
            {
                var products = new List<string>();
                foreach (XmlNode productNode in productNodes)
                {
                    if (productNode != null && productNode.HasChildNodes && String.IsNullOrEmpty(productNode.FirstChild?.InnerText) == false)
                    {
                        products.Add(productNode.FirstChild?.InnerText);
                    }
                }

                products.Sort();
                Product = products.ToArray();
            }

            var terminalIdentifierNode = node.SelectSingleNode("terminalIdentifier", mgr);
            if (terminalIdentifierNode != null && terminalIdentifierNode.HasChildNodes)
            {
                TerminalIdentifier = terminalIdentifierNode.FirstChild?.InnerText ?? string.Empty;
            }

            var sMDGTerminalCodeNode = node.SelectSingleNode("sMDGTerminalCode", mgr);
            if (sMDGTerminalCodeNode != null && sMDGTerminalCodeNode.HasChildNodes)
            {
                SMDGTerminalCode = sMDGTerminalCodeNode.FirstChild?.InnerText ?? string.Empty;
            }

            var uNLocationCodeNode = node.SelectSingleNode("uNLocationCode", mgr);
            if (uNLocationCodeNode != null && uNLocationCodeNode.HasChildNodes)
            {
                UNLocationCode = uNLocationCodeNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
