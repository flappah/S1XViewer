using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Features
{
    public abstract class AbstractRxn : InformationFeatureBase, IAbstractRxN
    {
        public string CategoryOfAuthority { get; set; } = string.Empty;
        public IGraphic[] Graphic { get; set; } = Array.Empty<Graphic>();
        public IRxnCode[] RxnCode { get; set; } = Array.Empty<RxnCode>();
        public ITextContent[] TextContent { get; set; }  = Array.Empty<TextContent>();

    }
}
