using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class ContactAddress : ComplexTypeBase, IContactAddress
    {
        public string[] DeliveryPoint { get; set; }
        public string CityName { get; set; }
        public string AdministrativeDivision { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new ContactAddress
            {
                DeliveryPoint = DeliveryPoint == null
                    ? new string[0]
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
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var deliveryPointNodes = node.SelectNodes("deliveryPoint", mgr);
            if (deliveryPointNodes != null && deliveryPointNodes.Count > 0)
            {
                var deliveryPoints = new List<string>();
                foreach (XmlNode deliveryPointNode in deliveryPointNodes)
                {
                    if (deliveryPointNode != null && deliveryPointNode.HasChildNodes)
                    {
                        deliveryPoints.Add(deliveryPointNode.FirstChild.InnerText);
                    }
                }
                DeliveryPoint = deliveryPoints.ToArray();
            }

            var cityNameNode = node.SelectSingleNode("cityName", mgr);
            if (cityNameNode != null && cityNameNode.HasChildNodes)
            {
                CityName = cityNameNode.FirstChild.InnerText;
            }

            var administrativeDivisionNode = node.SelectSingleNode("administrativeDivision", mgr);
            if (administrativeDivisionNode != null && administrativeDivisionNode.HasChildNodes)
            {
                AdministrativeDivision = administrativeDivisionNode.FirstChild.InnerText;
            }

            var countryNode = node.SelectSingleNode("country", mgr);
            if (countryNode != null && countryNode.HasChildNodes)
            {
                Country = countryNode.FirstChild.InnerText;
            }

            var postalCodeNode = node.SelectSingleNode("postalCode", mgr);
            if (postalCodeNode != null && postalCodeNode.HasChildNodes)
            {
                PostalCode = postalCodeNode.FirstChild.InnerText;
            }

            return this;
        }
    }
}
