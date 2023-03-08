using S1XViewer.Types.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.Types.Interfaces
{
    public interface ICurrentNonGravitational : IGeoFeature
    {
        public Orientation Orientation { get; set; }
        public Speed Speed { get; set; }
        string Status { get; set; }  
    }
}
