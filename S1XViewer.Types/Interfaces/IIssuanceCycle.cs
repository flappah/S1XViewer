using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IIssuanceCycle
    {
        IPeriodicDateRange PeriodicDateRange { get; set; }
        ITimeIntervalOfCycle TimeIntervalOfCycle { get; set; }

        IComplexType DeepClone();
        IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr);
    }
}