using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class Graphic : ComplexTypeBase, IGraphic
    {
        public string[] PictorialRepresentation { get; set; } = Array.Empty<string>();
        public string PictureCaption { get; set; } = string.Empty;
        public string SourceDate { get; set; } = string.Empty;  
        public string PictureInformation { get; set; } = string.Empty;
        public IBearingInformation BearingInformation { get; set; } = new BearingInformation();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new Graphic
            {
                PictorialRepresentation = PictorialRepresentation == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(PictorialRepresentation, s => s),
                PictureCaption = PictureCaption,
                SourceDate = SourceDate,
                PictureInformation = PictureInformation,
                BearingInformation = BearingInformation == null
                    ? new BearingInformation()
                    : BearingInformation.DeepClone() as IBearingInformation
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
            var pictorialRepresentationNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}pictorialRepresentation", mgr);
            if (pictorialRepresentationNodes != null && pictorialRepresentationNodes.Count > 0)
            {
                var representations = new List<string>();
                foreach(XmlNode pictorialRepresentationNode in pictorialRepresentationNodes)
                {
                    if (pictorialRepresentationNode != null && pictorialRepresentationNode.HasChildNodes)
                    {
                        var representation = pictorialRepresentationNode.FirstChild?.InnerText ?? string.Empty;
                        representations.Add(representation);
                    }
                }
                PictorialRepresentation = representations.ToArray();
            }

            var pictureCaptionNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}pictureCaptionNode", mgr);
            if (pictureCaptionNode != null && pictureCaptionNode.HasChildNodes)
            {
                PictureCaption = pictureCaptionNode.FirstChild?.InnerText ?? string.Empty;
            }

            var sourceDateNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}sourceDate", mgr);
            if (sourceDateNode != null && sourceDateNode.HasChildNodes)
            {
                SourceDate = sourceDateNode.FirstChild?.InnerText ?? string.Empty;
            }

            var pictureInformationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}pictureInformation", mgr);
            if (pictureInformationNode != null && pictureInformationNode.HasChildNodes)
            {
                PictureInformation = pictureInformationNode.FirstChild?.InnerText ?? string.Empty;
            }

            var bearingInformationNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}bearingInformation", mgr);
            if (bearingInformationNode != null && bearingInformationNode.HasChildNodes)
            {
                var bearingInformation = new BearingInformation();
                bearingInformation.FromXml(bearingInformationNode, mgr, nameSpacePrefix);
                BearingInformation = bearingInformation;
            }

            return this;
        }
    }
}
