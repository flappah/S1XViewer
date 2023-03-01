namespace S1XViewer.Types.Interfaces
{
    public interface IContactAddress : IComplexType
    {
        string AdministrativeDivision { get; set; }
        string CityName { get; set; }
        string Country { get; set; }
        string[] DeliveryPoint { get; set; }
        string PostalCode { get; set; }
    }
}