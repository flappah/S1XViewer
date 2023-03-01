using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.Types.Features
{
    public abstract class AbstractRxn : InformationFeatureBase, IAbstractRxN
    {
        public string CategoryOfAuthority { get; set; }
        public IGraphic[] Graphic { get; set; }
        public IRxnCode[] RxnCode { get; set; }
        public ITextContent[] TextContent { get; set; }

    }
}
