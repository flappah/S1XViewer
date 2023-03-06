using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.Types.Interfaces
{
    public interface ITidalStream : IGeoFeature
    {
        string[] CategoryOfTidalStream { get; set; }
        double OrientationUncertainty { get; set; }
        double OrientationValue { get; set; }
        double SpeedMaximum { get; set; }
        double SpeedMinimum { get; set; }
    }
}
