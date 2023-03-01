using System.Xml;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Interfaces
{
    public interface IPilotService : IGeoFeature
    {
        string[] CategoryOfPilot { get; set; }
        INoticeTime NoticeTime { get; set; }
        string PilotQualification { get; set; }
        string PilotRequest { get; set; }
        string RemotePilot { get; set; }
    }
}