using S1XViewer.Types.ComplexTypes;

namespace S1XViewer.Types.Interfaces
{
    public interface IOuterLimit : ILayout
    {
        ILimitsDescription LimitsDescription { get; set; }
        IMarkedBy[] MarkedBy { get; set; } 
        ILandmarkDescription[] LandmarkDescription { get; set; }
        IOffShoreMarkDescription[] OffShoreMarkDescription { get; set; } 
        IMajorLightDescription[] MajorLightDescription { get; set; } 
        IUsefulMarkDescription[] UsefulMarkDescription { get; set; } 
    }
}