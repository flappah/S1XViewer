using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class GeneralHarbourInformation : ComplexTypeBase, IGeneralHarbourInformation
    {
        public IGeneralPortDescription GeneralPortDescription { get; set; } = new GeneralPortDescription();
        public IFacilitiesLayoutDescription FacilitiesLayoutDescription { get; set; } = new FacilitiesLayoutDescription();
        public ILimitsDescription LimitsDescription { get; set; } = new LimitsDescription();
        public IConstructionInformation ConstructionInformation { get; set; } = new ConstructionInformation();
        public ICargoServicesDescription CargoServicesDescription { get; set; } = new CargoServicesDescription();
        public IWeatherResource[] WeatherResource { get; set; } = Array.Empty<WeatherResource>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new GeneralHarbourInformation()
            {
                GeneralPortDescription = GeneralPortDescription == null ? new GeneralPortDescription() : GeneralPortDescription.DeepClone() as IGeneralPortDescription,
                FacilitiesLayoutDescription = FacilitiesLayoutDescription == null ? new FacilitiesLayoutDescription() : FacilitiesLayoutDescription.DeepClone() as IFacilitiesLayoutDescription,
                LimitsDescription = LimitsDescription == null ? new LimitsDescription() : LimitsDescription.DeepClone() as ILimitsDescription,
                ConstructionInformation = ConstructionInformation == null ? new ConstructionInformation() : ConstructionInformation.DeepClone() as IConstructionInformation,
                CargoServicesDescription = CargoServicesDescription == null ? new CargoServicesDescription() : CargoServicesDescription.DeepClone() as ICargoServicesDescription,
                WeatherResource = WeatherResource == null
                    ? new WeatherResource[0]
                    : Array.ConvertAll(WeatherResource, t => t.DeepClone() as IWeatherResource),
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
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            var generalPortDescriptionNode = node.SelectSingleNode("generalPortDescription", mgr);
            if (generalPortDescriptionNode != null && generalPortDescriptionNode.HasChildNodes)
            {
                GeneralPortDescription = new GeneralPortDescription();
                GeneralPortDescription.FromXml(generalPortDescriptionNode, mgr);
            }

            var facilitiesLayoutDescriptionNode = node.SelectSingleNode("facilitiesLayoutDescription", mgr);
            if (facilitiesLayoutDescriptionNode != null && facilitiesLayoutDescriptionNode.HasChildNodes)
            {
                FacilitiesLayoutDescription = new FacilitiesLayoutDescription();
                FacilitiesLayoutDescription.FromXml(facilitiesLayoutDescriptionNode, mgr);
            }

            var limitsDescriptionNode = node.SelectSingleNode("limitsDescription", mgr);
            if (limitsDescriptionNode != null && limitsDescriptionNode.HasChildNodes)
            {
                LimitsDescription = new LimitsDescription();
                LimitsDescription.FromXml(limitsDescriptionNode, mgr);
            }

            var constructionInformationNode = node.SelectSingleNode("constructionInformation", mgr);
            if (constructionInformationNode != null && constructionInformationNode.HasChildNodes)
            {
                ConstructionInformation = new ConstructionInformation();
                ConstructionInformation.FromXml(constructionInformationNode, mgr);
            }

            var cargoServicesDescriptionNode = node.SelectSingleNode("cargoServicesDescription", mgr);
            if (cargoServicesDescriptionNode != null && cargoServicesDescriptionNode.HasChildNodes)
            {
                CargoServicesDescription = new CargoServicesDescription();
                CargoServicesDescription.FromXml(cargoServicesDescriptionNode, mgr);
            }

            var weatherResourceNodes = node.SelectNodes("weatherResource", mgr);
            if (weatherResourceNodes != null && weatherResourceNodes.Count > 0)
            {
                var weatherResources = new List<WeatherResource>();
                foreach (XmlNode weatherResourceNode in weatherResourceNodes)
                {
                    if (weatherResourceNode != null && weatherResourceNode.HasChildNodes)
                    {
                        var newWeatherResource = new WeatherResource();
                        newWeatherResource.FromXml(weatherResourceNode, mgr);
                        weatherResources.Add(newWeatherResource);
                    }
                }

                WeatherResource = weatherResources.ToArray();
            }

            return this;
        }
    }
}
