using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IGeneralHarbourInformation : IComplexType
    {
        ICargoServicesDescription CargoServicesDescription { get; set; }
        IConstructionInformation ConstructionInformation { get; set; }
        IFacilitiesLayoutDescription FacilitiesLayoutDescription { get; set; }
        IGeneralPortDescription GeneralPortDescription { get; set; }
        ILimitsDescription LimitsDescription { get; set; }
        IWeatherResource[] WeatherResource { get; set; }
    }
}