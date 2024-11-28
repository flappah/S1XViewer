using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System;
using System.Collections.Generic;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class PilotService : GeoFeatureBase, IPilotService, IS127Feature
    {
        public string[] CategoryOfPilot { get; set; } = Array.Empty<string>();
        public string PilotQualification { get; set; } = string.Empty;
        public string PilotRequest { get; set; } = string.Empty;
        public string RemotePilot { get; set; } = string.Empty;
        public INoticeTime NoticeTime { get; set; } = new NoticeTime();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new PilotService
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
                CategoryOfPilot = CategoryOfPilot == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(CategoryOfPilot, s => s),
                PilotQualification = PilotQualification,
                PilotRequest = PilotRequest,
                RemotePilot = RemotePilot,
                NoticeTime = NoticeTime == null
                    ? new NoticeTime()
                    : NoticeTime.DeepClone() as INoticeTime,
                Links = Links == null
                    ? Array.Empty<Link>()
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

            var categoryOfPilotNodes = node.SelectNodes("categoryOfPilot", mgr);
            if (categoryOfPilotNodes != null && categoryOfPilotNodes.Count > 0)
            {
                var categories = new List<string>();
                foreach (XmlNode categoryOfPilotNode in categoryOfPilotNodes)
                {
                    var category = categoryOfPilotNode.FirstChild?.InnerText ?? string.Empty;
                    categories.Add(category);
                }
                CategoryOfPilot = categories.ToArray();
            }

            var pilotQualificationNode = node.SelectSingleNode("pilotQualification", mgr);
            if (pilotQualificationNode != null && pilotQualificationNode.HasChildNodes)
            {
                PilotQualification = pilotQualificationNode.FirstChild?.InnerText ?? string.Empty;
            }

            var pilotRequestNode = node.SelectSingleNode("pilotRequest", mgr);
            if (pilotRequestNode != null && pilotRequestNode.HasChildNodes)
            {
                PilotRequest = pilotRequestNode.FirstChild?.InnerText ?? string.Empty;
            }

            var remotePilotNode = node.SelectSingleNode("remotePilot", mgr);
            if (remotePilotNode != null && remotePilotNode.HasChildNodes)
            {
                RemotePilot = remotePilotNode.FirstChild?.InnerText ?? string.Empty;
            }

            var noticeTimeNode = node.SelectSingleNode("noticeTime", mgr);
            if (noticeTimeNode != null && noticeTimeNode.HasChildNodes)
            {
                NoticeTime = new NoticeTime();
                NoticeTime.FromXml(noticeTimeNode, mgr);
            }

            return this;
        }

        /// <summary>
        ///     Generates the feature code necessary for portrayal
        /// </summary>
        /// <returns></returns>
        public override string GetSymbolName()
        {
            return "";
        }
    }
}
