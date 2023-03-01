namespace S1XViewer.Types.Interfaces
{
    public interface IOnlineResource : IComplexType
    {
        string ApplicationProfile { get; set; }
        string Linkage { get; set; }
        string NameOfResource { get; set; }
        string OnlineDescription { get; set; }
        string OnlineFunction { get; set; }
        string Protocol { get; set; }
        string ProtocolRequest { get; set; }
    }
}