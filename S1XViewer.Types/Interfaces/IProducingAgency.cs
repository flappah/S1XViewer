namespace S1XViewer.Types.Interfaces
{
    public interface IProducingAgency : IComplexType
    {
        IContactAddress ContactAddress { get; set; }
        string IndividualName { get; set; }
        IOnlineResource OnlineResource { get; set; }
        string OrganizationName { get; set; }
        string PositionName { get; set; }
        ITelecommunications Telecommunications { get; set; }
    }
}