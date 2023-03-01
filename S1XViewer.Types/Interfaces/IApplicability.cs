namespace S1XViewer.Types.Interfaces
{
    public interface IApplicability : IInformationFeature
    {
        string Ballast { get; set; }
        string[] CategoryOfCargo { get; set; }
        string[] CategoryOfDangerousOrHazardousCargo { get; set; }
        string CategoryOfVessel { get; set; }
        string CategoryOfVesselRegistry { get; set; }
        string LogicalConnectives { get; set; }
        string ThicknessOfIceCapability { get; set; }
        IVesselsMeasurement[] VesselsMeasurements { get; set; }
        string VesselPerformance { get; set; }
    }
}