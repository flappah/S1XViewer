using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S131DataPackage : XmlDataPackage, IS128DataPackage
    {
        public S131DataPackage()
        {
            Id = Guid.NewGuid();
        }
    }
}
