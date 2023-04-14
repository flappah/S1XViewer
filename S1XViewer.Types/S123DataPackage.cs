using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S123DataPackage : XmlDataPackage, IS123DataPackage
    {
        public S123DataPackage()
        {
            Id = Guid.NewGuid();
        }
    }
}
