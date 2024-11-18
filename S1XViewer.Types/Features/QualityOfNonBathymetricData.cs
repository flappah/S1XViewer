using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class QualityOfNonBathymetricData : QualityOfTemporalVariation, IQualityOfNonBathymetricData, IS122Feature, IS123Feature, IS127Feature
    {
        public string DataAssessment { get; set; }
        public ISourceIndication SourceIndication { get; set; }
        public string[] HorizontalDistanceUncertainty { get; set; }
        public IHorizontalPositionalUncertainty HorizontalPositionalUncertainty { get; set; }
        public string DirectionUncertainty { get; set; }
        public ISurveyDateRange SurveyDateRange { get; set; }

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

            if (node.HasChildNodes)
            {
                if (node.Attributes?.Count > 0 &&
                    node.Attributes.Contains("gml:id") == true)
                {
                    Id = node.Attributes["gml:id"].InnerText;
                }
            }

            var featureObjectIdentifierNode = node.SelectSingleNode("S100:featureObjectIdentifier", mgr);
            if (featureObjectIdentifierNode != null && featureObjectIdentifierNode.HasChildNodes)
            {
                FeatureObjectIdentifier = new FeatureObjectIdentifier();
                FeatureObjectIdentifier.FromXml(featureObjectIdentifierNode, mgr);
            }

            var dataAssessment = node.SelectSingleNode("dataAssessment", mgr);
            if (dataAssessment != null)
            {
                DataAssessment = dataAssessment.InnerText;
            }

            var sourceIndicationNode = node.SelectSingleNode("sourceIndication", mgr);
            if (sourceIndicationNode != null && sourceIndicationNode.HasChildNodes)
            {
                SourceIndication = new SourceIndication();
                SourceIndication.FromXml(sourceIndicationNode, mgr);
            }

            var horizontalDistanceUncertaintyNodes = node.SelectNodes("horizontalDistanceUncertainty", mgr);
            if (horizontalDistanceUncertaintyNodes != null && horizontalDistanceUncertaintyNodes.Count > 0)
            {
                var distanceUncertainties = new List<string>();
                foreach (XmlNode horizontalDistanceUncertaintyNode in horizontalDistanceUncertaintyNodes)
                {
                    if (horizontalDistanceUncertaintyNode != null && horizontalDistanceUncertaintyNode.HasChildNodes)
                    {
                        distanceUncertainties.Add(horizontalDistanceUncertaintyNode.FirstChild.InnerText);
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
                DirectionUncertainty = directionUncertaintyNode.FirstChild.InnerText;
            }

            var surveyDateRangeNode = node.SelectSingleNode("surveyDateRange", mgr);
            if (surveyDateRangeNode != null && surveyDateRangeNode.HasChildNodes)
            {
                SurveyDateRange = new SurveyDateRange();
                SurveyDateRange.FromXml(surveyDateRangeNode, mgr);
            }

            var categoryOfTemporalVariation = node.SelectSingleNode("categoryOfTemporalVariation", mgr);
            if (categoryOfTemporalVariation != null)
            {
                CategoryOfTemporalVariation = categoryOfTemporalVariation.InnerText;
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
