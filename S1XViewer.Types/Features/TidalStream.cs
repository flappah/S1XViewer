using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class TidalStream : GeoFeatureBase, ITidalStream, IS111Feature
    {
        public string[] CategoryOfTidalStream { get; set; } = new string[0];
        public double OrientationUncertainty { get; set; }
        public double OrientationValue { get; set; }
        public double SpeedMaximum { get; set; }
        public double SpeedMinimum { get; set; }

        public override IFeature DeepClone()
        {
            throw new NotImplementedException();
        }

        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            throw new NotImplementedException();
        }
    }
}
