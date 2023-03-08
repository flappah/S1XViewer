namespace S1XViewer.Types.Interfaces
{
    public interface IOrientation : IComplexType
    {
        double OrientationUncertainty { get; set; }
        double OrientationValue { get; set; }
    }
}