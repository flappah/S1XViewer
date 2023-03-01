using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public abstract class InformationFeatureBase : FeatureBase, IInformationFeature
    {
        public IFeatureName[] FeatureName { get; set; }
        public IDateRange FixedDateRange { get; set; }
        public IDateRange[] PeriodicDateRange { get; set; }
        public ISourceIndication[] SourceIndication { get; set; }
    }
}
