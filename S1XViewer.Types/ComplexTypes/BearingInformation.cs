using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class BearingInformation : ComplexTypeBase, IBearingInformation
    {
        public string CardinalDirection { get; set; }
        public string Distance { get; set; }
        public IInformation[] Information { get; set; }
        public IOrientation Orientation { get; set; }
        public string[] SectorBearing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new BearingInformation
            {
                CardinalDirection = CardinalDirection,
                Distance = Distance,
                Information = Information == null
                    ? new Information[0]
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                Orientation = Orientation == null
                    ? new Orientation()
                    : Orientation.DeepClone() as IOrientation,
                SectorBearing = SectorBearing == null
                    ? new string[0]
                    : Array.ConvertAll(SectorBearing, sb => sb)
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
            var cardinalDirectionNode = node.SelectSingleNode("cardinalDirection", mgr);
            if (cardinalDirectionNode != null && cardinalDirectionNode.HasChildNodes)
            {
                CardinalDirection = cardinalDirectionNode.FirstChild.InnerText;
            }

            var distanceNode = node.SelectSingleNode("distance", mgr);
            if (distanceNode != null && distanceNode.HasChildNodes)
            {
                Distance = distanceNode.FirstChild.InnerText;
            }

            var informationNodes = node.SelectNodes("information", mgr);
            if (informationNodes != null && informationNodes.Count > 0)
            {
                var informations = new List<Information>();
                foreach (XmlNode informationNode in informationNodes)
                {
                    if (informationNode != null && informationNode.HasChildNodes)
                    {
                        var newInformation = new Information();
                        newInformation.FromXml(informationNode, mgr);
                        informations.Add(newInformation);
                    }
                }
                Information = informations.ToArray();
            }

            var orientationNode = node.SelectSingleNode("orientation", mgr);
            if (orientationNode != null && orientationNode.HasChildNodes)
            {
                Orientation = new Orientation();
                Orientation.FromXml(orientationNode, mgr);
            }

            var sectorBearingNodes = node.SelectNodes("sectorBearing", mgr);
            if (sectorBearingNodes != null && sectorBearingNodes.Count > 0)
            {
                var bearings = new List<string>();
                foreach(XmlNode sectorBearingNode in sectorBearingNodes)
                {
                    if (sectorBearingNode != null && sectorBearingNode.HasChildNodes)
                    {
                        bearings.Add(sectorBearingNode.FirstChild.InnerText);
                    }
                }
                SectorBearing = bearings.ToArray();
            }

            return this;
        }
    }
}
