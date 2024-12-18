﻿using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class SourceIndication : ComplexTypeBase, ISourceIndication
    {
        public string CategoryOfAuthority { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public IFeatureName[] FeatureName { get; set; } = Array.Empty<FeatureName>();
        public string ReportedDate { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new SourceIndication
            {
                CategoryOfAuthority = CategoryOfAuthority,
                Country = Country,
                FeatureName = FeatureName == null
                    ? new[] { new FeatureName() }
                    : Array.ConvertAll(FeatureName, fn => fn.DeepClone() as IFeatureName),
                ReportedDate = ReportedDate,
                Source = Source,
                SourceType = SourceType
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            if (node != null && node.HasChildNodes)
            {
                var categoryOfAuthority = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}categoryOfAuthority", mgr);
                if (categoryOfAuthority != null)
                {
                    CategoryOfAuthority = categoryOfAuthority.InnerText;
                }

                var country = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}country", mgr);
                if (country != null)
                {
                    Country = country.InnerText;
                }

                var featureNameNodes = node.SelectNodes($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}featureName", mgr);
                if (featureNameNodes != null && featureNameNodes.Count > 0)
                {
                    var featureNames = new List<FeatureName>();
                    foreach (XmlNode featureNameNode in featureNameNodes)
                    {
                        var newFeatureName = new FeatureName();
                        newFeatureName.FromXml(featureNameNode, mgr, nameSpacePrefix);
                        featureNames.Add(newFeatureName);
                    }
                    FeatureName = featureNames.ToArray();
                }

                var reportedDateNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}reportedDate", mgr);
                if (reportedDateNode != null && reportedDateNode.HasChildNodes)
                {
                    ReportedDate = reportedDateNode.FirstChild?.InnerText ?? string.Empty;
                }

                var source = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}source", mgr);
                if (source != null)
                {
                    Source = source.InnerText;
                }

                var sourceType = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}sourceType", mgr);
                if (sourceType != null)
                {
                    SourceType = sourceType.InnerText;
                }
            }

            return this;
        }

    }
}
