namespace S1XViewer.Types.Interfaces
{
    public interface IUnderkeelAllowance : IComplexType
    {
        string UnderkeelAllowanceFixed { get; set; }
        string UnderkeelAllowanceVariableBeamBased { get; set; }
        string UnderkeelAllowanceVariableDraughtBased { get; set; }
        string Operation { get; set; }
}
}