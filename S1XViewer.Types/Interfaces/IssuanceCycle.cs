using S1XViewer.Types.ComplexTypes;
using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public class IssuanceCycle : ComplexTypeBase, IIssuanceCycle
    {
        public IPeriodicDateRange PeriodicDateRange { get; set; } = new PeriodicDateRange();
        public ITimeIntervalOfCycle TimeIntervalOfCycle { get; set; } = new TimeIntervalOfCycle();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new IssuanceCycle()
            {
                PeriodicDateRange = PeriodicDateRange == null ? new PeriodicDateRange() : PeriodicDateRange.DeepClone() as IPeriodicDateRange,
                TimeIntervalOfCycle = TimeIntervalOfCycle == null ? new TimeIntervalOfCycle() : TimeIntervalOfCycle.DeepClone() as ITimeIntervalOfCycle
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
            //public IPeriodicDateRange PeriodicDateRange { get; set; } = new PeriodicDateRange();
            var periodicDateRangeNode = node.SelectSingleNode("periodicDateRange", mgr);
            if (periodicDateRangeNode == null)
            {
                periodicDateRangeNode = node.SelectSingleNode("S128:periodicDateRange", mgr);
            }
            if (periodicDateRangeNode != null && periodicDateRangeNode.HasChildNodes)
            {
                PeriodicDateRange = new PeriodicDateRange();
                PeriodicDateRange.FromXml(periodicDateRangeNode, mgr);
            }

            //public ITimeIntervalOfCycle TimeIntervalOfCycle { get; set; } = new TimeIntervalOfCycle();
            var timeIntervalOfCycleNode = node.SelectSingleNode("timeIntervalOfCycle", mgr);
            if (timeIntervalOfCycleNode == null)
            {
                timeIntervalOfCycleNode = node.SelectSingleNode("S128:timeIntervalOfCycle", mgr);
            }
            if (timeIntervalOfCycleNode != null && timeIntervalOfCycleNode.HasChildNodes)
            {
                TimeIntervalOfCycle = new TimeIntervalOfCycle();
                TimeIntervalOfCycle.FromXml(timeIntervalOfCycleNode, mgr);
            }

            return this;
        }
    }
}
