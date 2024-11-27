using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class NonStandardWorkingDay : InformationFeatureBase, INonStandardWorkingDay, IS122Feature, IS127Feature
    {
        public string[] DateFixed { get; set; } = Array.Empty<string>();
        public string[] DateVariable { get; set; } = Array.Empty<string>();
        public IInformation[] Information { get; set; } = Array.Empty<Information>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new NonStandardWorkingDay
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
                    ? new SourceIndication[0]
                    : Array.ConvertAll(SourceIndication, s => s.DeepClone() as ISourceIndication),
                DateFixed = DateFixed == null
                    ? Array.Empty<string>()
                    : Array.ConvertAll(DateFixed, s => s),
                DateVariable = DateVariable == null
                    ? new string[0]
                    : Array.ConvertAll(DateVariable, dv => dv),
                Information = Information == null
                    ? Array.Empty<Information>()
                    : Array.ConvertAll(Information, i => i.DeepClone() as IInformation),
                Links = Links == null
                    ? Array.Empty<Link>()
                    : Array.ConvertAll(Links, l => l.DeepClone() as ILink)
            };
        }

        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;
            base.FromXml(node, mgr );

            var dateFixedNodes = node.SelectNodes("dateFixed", mgr);
            if (dateFixedNodes != null && dateFixedNodes.Count > 0)
            {
                var datesFixed = new List<string>();
                foreach (XmlNode dateFixedNode in dateFixedNodes)
                {
                    if (dateFixedNode != null && dateFixedNode.HasChildNodes)
                    {
                        datesFixed.Add(dateFixedNode.FirstChild.InnerText);
                    }
                }
                DateFixed = datesFixed.ToArray();
            }

            var dateVariableNodes = node.SelectNodes("dateVariable", mgr);
            if (dateVariableNodes != null && dateVariableNodes.Count > 0)
            {
                var datesVariable = new List<string>();
                foreach (XmlNode dateVariableNode in dateVariableNodes)
                {
                    if (dateVariableNode != null && dateVariableNode.HasChildNodes)
                    {
                        datesVariable.Add(dateVariableNode.FirstChild.InnerText);
                    }
                }
                DateVariable = datesVariable.ToArray();
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

            return this;
        }
    }
}
