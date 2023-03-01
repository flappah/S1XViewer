namespace S1XViewer.Types.Interfaces
{
    public interface IOrientation : IComplexType
    {
        string OrientationUncertainty { get; set; }
        string OrientationValue { get; set; }
    }
}