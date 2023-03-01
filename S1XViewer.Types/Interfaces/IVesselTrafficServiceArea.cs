namespace S1XViewer.Types.Interfaces
{
    public interface IVesselTrafficServiceArea : IGeoFeature
    {
        string CategoryOfVesselTrafficService { get; set; }
        string ServiceAccessProcedure { get; set; }
        string RequirementsForMaintenanceOfListeningWatch { get; set; }
    }
}