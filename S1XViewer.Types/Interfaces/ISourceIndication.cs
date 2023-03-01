using System;
using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface ISourceIndication : IComplexType
    {
        string CategoryOfAuthority { get; set; }
        string Country { get; set; }
        IFeatureName[] FeatureName { get; set; }
        string ReportedDate { get; set; }
        string Source { get; set; }
        string SourceType { get; set; }
    }
}