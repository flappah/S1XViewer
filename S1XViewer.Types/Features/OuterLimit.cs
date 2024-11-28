using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class OuterLimit : Layout, IOuterLimit
    {
        public ILimitsDescription LimitsDescription { get; set; } = new LimitsDescription();
        public IMarkedBy[] MarkedBy { get; set; } = Array.Empty<MarkedBy>();
        public ILandmarkDescription[] LandmarkDescription { get; set; } = Array.Empty<LandmarkDescription>();
        public IOffShoreMarkDescription[] OffShoreMarkDescription { get; set; } = Array.Empty<OffShoreMarkDescription>();
        public IMajorLightDescription[] MajorLightDescription { get; set; } = Array.Empty<MajorLightDescription>();
        public IUsefulMarkDescription[] UsefulMarkDescription { get; set; } = Array.Empty<UsefulMarkDescription>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new OuterLimit()
            {
                LimitsDescription = LimitsDescription == null ? new LimitsDescription() : LimitsDescription.DeepClone() as ILimitsDescription,
                MarkedBy = MarkedBy == null ? Array.Empty<MarkedBy>() : Array.ConvertAll(MarkedBy, m => m),
                LandmarkDescription = LandmarkDescription == null ? Array.Empty<LandmarkDescription>() : Array.ConvertAll(LandmarkDescription, l => l.DeepClone() as ILandmarkDescription),
                OffShoreMarkDescription = OffShoreMarkDescription == null ? Array.Empty<OffShoreMarkDescription>() : Array.ConvertAll(OffShoreMarkDescription, l => l.DeepClone() as IOffShoreMarkDescription),
                MajorLightDescription = MajorLightDescription == null ? Array.Empty<MajorLightDescription>() : Array.ConvertAll(MajorLightDescription, l => l.DeepClone() as IMajorLightDescription),
                UsefulMarkDescription = UsefulMarkDescription == null ? Array.Empty<UsefulMarkDescription>() : Array.ConvertAll(UsefulMarkDescription, l => l.DeepClone() as IUsefulMarkDescription),
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

            var limitsDescriptionNode = node.SelectSingleNode("limitsDescription", mgr);
            if (limitsDescriptionNode != null && limitsDescriptionNode.HasChildNodes)
            {
                LimitsDescription = new LimitsDescription();
                LimitsDescription.FromXml(limitsDescriptionNode, mgr);
            }

            var markedByNodes = node.SelectNodes("markedBy", mgr);
            if (markedByNodes != null && markedByNodes.Count > 0)
            {
                var markedByItems = new List<MarkedBy>();
                foreach (XmlNode markedByNode in markedByNodes)
                {
                    if (markedByNode != null && markedByNode.HasChildNodes)
                    {
                        var markedBy = new MarkedBy();
                        markedBy.FromXml(markedByNode, mgr);
                        markedByItems.Add(markedBy);
                    }
                }

                MarkedBy = markedByItems.ToArray();
            }

            var landmarkDescriptionNodes = node.SelectNodes("landmarkDescription", mgr);
            if (landmarkDescriptionNodes != null && landmarkDescriptionNodes.Count > 0)
            {
                var descriptions = new List<LandmarkDescription>();
                foreach (XmlNode landmarkDescriptionNode in landmarkDescriptionNodes)
                {
                    if (landmarkDescriptionNode != null && landmarkDescriptionNode.HasChildNodes)
                    {
                        var newLandmarkDescription = new LandmarkDescription();
                        newLandmarkDescription.FromXml(landmarkDescriptionNode, mgr);
                        descriptions.Add(newLandmarkDescription);
                    }
                }
                LandmarkDescription = descriptions.ToArray();
            }

            var offshoreMarkDescriptionNodes = node.SelectNodes("offshoreMarkDescription", mgr);
            if (offshoreMarkDescriptionNodes != null && offshoreMarkDescriptionNodes.Count > 0)
            {
                var descriptions = new List<OffShoreMarkDescription>();
                foreach (XmlNode offshoreMarkDescriptionNode in offshoreMarkDescriptionNodes)
                {
                    if (offshoreMarkDescriptionNode != null && offshoreMarkDescriptionNode.HasChildNodes)
                    {
                        var newOffShoreMarkDescription = new OffShoreMarkDescription();
                        newOffShoreMarkDescription.FromXml(offshoreMarkDescriptionNode, mgr);
                        descriptions.Add(newOffShoreMarkDescription);
                    }
                }
                OffShoreMarkDescription = descriptions.ToArray();
            }

            var majorLightDescriptionNodes = node.SelectNodes("majorLightDescription", mgr);
            if (majorLightDescriptionNodes != null && majorLightDescriptionNodes.Count > 0)
            {
                var descriptions = new List<MajorLightDescription>();
                foreach (XmlNode majorLightDescriptionNode in majorLightDescriptionNodes)
                {
                    if (majorLightDescriptionNode != null && majorLightDescriptionNode.HasChildNodes)
                    {
                        var newMajorLightDescription = new MajorLightDescription();
                        newMajorLightDescription.FromXml(majorLightDescriptionNode, mgr);
                        descriptions.Add(newMajorLightDescription);
                    }
                }
                MajorLightDescription = descriptions.ToArray();
            }

            var usefulMarkDescriptionNodes = node.SelectNodes("usefulMarkDescription", mgr);
            if (usefulMarkDescriptionNodes != null && usefulMarkDescriptionNodes.Count > 0)
            {
                var descriptions = new List<UsefulMarkDescription>();
                foreach (XmlNode usefulMarkDescriptionNode in usefulMarkDescriptionNodes)
                {
                    if (usefulMarkDescriptionNode != null && usefulMarkDescriptionNode.HasChildNodes)
                    {
                        var newUsefulMarkDescription = new UsefulMarkDescription();
                        newUsefulMarkDescription.FromXml(usefulMarkDescriptionNode, mgr);
                        descriptions.Add(newUsefulMarkDescription);
                    }
                }
                UsefulMarkDescription = descriptions.ToArray();
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
