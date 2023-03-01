namespace S1XViewer.Types.Interfaces
{
    public interface IVesselsMeasurement : IComplexType
    {
        string ComparisonOperator { get; set; }
        string VesselsCharacteristics { get; set; }
        string VesselsCharacteristicsUnit { get; set; }
        string VesselsCharacteristicsValue { get; set; }
    }
}