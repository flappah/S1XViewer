namespace S1XViewer.Types.Interfaces
{
    public interface IShipReportingServiceArea : IReportableServiceArea
    {
        string ServiceAccessProcedure { get; set; }
        string RequirementsForMaintenanceOfListeningWatch { get; set; }
    }
}
