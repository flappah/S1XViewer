namespace S1XViewer.Types.Interfaces
{
    public interface ITextPlacement : IGeoFeature
    {
        string FlipBearing { get; set; }
        string ScaleMinimum { get; set; }
        string Text { get; set; }
        string TextJustification { get; set; }
        string TextType { get; set; }
    }
}