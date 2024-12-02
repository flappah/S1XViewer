using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class ContactAddress : ComplexTypeBase, IContactAddress
    {
        public string[] DeliveryPoint { get; set; } = Array.Empty<string>();
        public string CityName { get; set; } = string.Empty;
        public string AdministrativeDivision { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new ContactAddress
            {
                DeliveryPoint = DeliveryPoint == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(DeliveryPoint, s => s),
                CityName = CityName,
                AdministrativeDivision = AdministrativeDivision,
                Country = Country,
                PostalCode = PostalCode
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var deliveryPointNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}deliveryPoint", mgr);
            if (deliveryPointNodes != null && deliveryPointNodes.Count > 0)
            {
                var deliveryPoints = new List<string>();
                foreach (XmlNode deliveryPointNode in deliveryPointNodes)
                {
                    if (deliveryPointNode != null && deliveryPointNode.HasChildNodes)
                    {
                        deliveryPoints.Add(deliveryPointNode.FirstChild?.InnerText ?? string.Empty);
                    }
                }
                DeliveryPoint = deliveryPoints.ToArray();
            }

            var cityNameNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}cityName", mgr);
            if (cityNameNode != null && cityNameNode.HasChildNodes)
            {
                CityName = cityNameNode.FirstChild?.InnerText ?? string.Empty;
            }

            var administrativeDivisionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}administrativeDivision", mgr);
            if (administrativeDivisionNode != null && administrativeDivisionNode.HasChildNodes)
            {
                AdministrativeDivision = administrativeDivisionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var countryNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}country", mgr);
            if (countryNode != null && countryNode.HasChildNodes)
            {
                Country = countryNode.FirstChild?.InnerText ?? string.Empty;
            }

            var postalCodeNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}postalCode", mgr);
            if (postalCodeNode != null && postalCodeNode.HasChildNodes)
            {
                PostalCode = postalCodeNode.FirstChild?.InnerText ?? string.Empty;
            }

            return this;
        }
    }
}
