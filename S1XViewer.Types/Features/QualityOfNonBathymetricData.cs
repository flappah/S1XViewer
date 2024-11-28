using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class QualityOfNonBathymetricData : QualityOfTemporalVariation, IQualityOfNonBathymetricData, IS122Feature, IS123Feature, IS127Feature, IS131Feature
    {
        public string DataAssessment { get; set; } = string.Empty;
        public ISourceIndication SourceIndication { get; set; } = new SourceIndication();
        public string[] HorizontalDistanceUncertainty { get; set; } = new string[0];
        public IHorizontalPositionalUncertainty HorizontalPositionalUncertainty { get; set; } = new HorizontalPositionalUncertainty();
        public string DirectionUncertainty { get; set; }  = string.Empty;
        public float OrientationUncertainty { get; set; } = 0.0f;
        public ISurveyDateRange SurveyDateRange { get; set; } = new SurveyDateRange();
        public IVerticalUncertainty VerticalUncertainty { get; set; } = new VerticalUncertainty();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new QualityOfNonBathymetricData
            {
                Information = Information == null
                    ? new Information[0]
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                CategoryOfTemporalVariation = CategoryOfTemporalVariation,
                FeatureObjectIdentifier = FeatureObjectIdentifier == null
                    ? new FeatureObjectIdentifier()
                    : FeatureObjectIdentifier.DeepClone() as IFeatureObjectIdentifier,
                DataAssessment = DataAssessment,
                SourceIndication = SourceIndication == null
                    ? new SourceIndication()
                    : SourceIndication.DeepClone() as ISourceIndication,
                HorizontalDistanceUncertainty = HorizontalDistanceUncertainty == null
                    ? new string[0]
                    : Array.ConvertAll(HorizontalDistanceUncertainty, hdu => hdu),
                HorizontalPositionalUncertainty = HorizontalPositionalUncertainty == null   
                    ? new HorizontalPositionalUncertainty()
                    : HorizontalPositionalUncertainty.DeepClone() as IHorizontalPositionalUncertainty,
                DirectionUncertainty = DirectionUncertainty,
                OrientationUncertainty = OrientationUncertainty,
                SurveyDateRange = SurveyDateRange == null   
                    ? new SurveyDateRange()
                    : SurveyDateRange.DeepClone() as ISurveyDateRange,
                Geometry = Geometry,
                Id = Id,
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
        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            base.FromXml(node, mgr);

            var dataAssessment = node.SelectSingleNode("dataAssessment", mgr);
            if (dataAssessment != null)
            {
                DataAssessment = dataAssessment.InnerText;
            }

            var horizontalDistanceUncertaintyNodes = node.SelectNodes("horizontalDistanceUncertainty", mgr);
            if (horizontalDistanceUncertaintyNodes != null && horizontalDistanceUncertaintyNodes.Count > 0)
            {
                var distanceUncertainties = new List<string>();
                foreach (XmlNode horizontalDistanceUncertaintyNode in horizontalDistanceUncertaintyNodes)
                {
                    if (horizontalDistanceUncertaintyNode != null && horizontalDistanceUncertaintyNode.HasChildNodes)
                    {
                        if (horizontalDistanceUncertaintyNode.FirstChild != null)
                        {
                            distanceUncertainties.Add(horizontalDistanceUncertaintyNode.FirstChild.InnerText);

                        }
                    }
                }
                HorizontalDistanceUncertainty = distanceUncertainties.ToArray();
            }

            var horizontalPositionalUncertaintyNode = node.SelectSingleNode("horizontalPositionalUncertainty", mgr);
            if (horizontalPositionalUncertaintyNode != null && horizontalPositionalUncertaintyNode.HasChildNodes)
            {
                HorizontalPositionalUncertainty = new HorizontalPositionalUncertainty();
                HorizontalPositionalUncertainty.FromXml(horizontalPositionalUncertaintyNode, mgr);
            }

            var directionUncertaintyNode = node.SelectSingleNode("directionUncertainty", mgr);
            if (directionUncertaintyNode != null && directionUncertaintyNode.HasChildNodes)
            {
                DirectionUncertainty = directionUncertaintyNode.FirstChild?.InnerText ?? string.Empty;
            }

            var orientationUncertaintyNode = node.SelectSingleNode("orientationUncertainty", mgr);
            if (orientationUncertaintyNode != null && orientationUncertaintyNode.HasChildNodes)
            {
                if (float.TryParse(orientationUncertaintyNode.InnerText, out float orientationUncertaintyValue))
                {
                    OrientationUncertainty = orientationUncertaintyValue;
                }
            }

            var verticalUncertaintyNode = node.SelectSingleNode("verticalUncertainty", mgr);
            if (verticalUncertaintyNode != null && verticalUncertaintyNode.HasChildNodes)
            {
                VerticalUncertainty = new VerticalUncertainty();
                VerticalUncertainty.FromXml(verticalUncertaintyNode, mgr); 
            }

            var sourceIndicationNode = node.SelectSingleNode("sourceIndication", mgr);
            if (sourceIndicationNode != null && sourceIndicationNode.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndicationNode, mgr);
            }

            var surveyDateRangeNode = node.SelectSingleNode("surveyDateRange", mgr);
            if (surveyDateRangeNode != null && surveyDateRangeNode.HasChildNodes)
            {
                SurveyDateRange = new SurveyDateRange();
                SurveyDateRange.FromXml(surveyDateRangeNode, mgr);
            }

            var linkNodes = node.SelectNodes("*[boolean(@xlink:href)]", mgr);
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
