using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S127DataPackage : XmlDataPackage, IS127DataPackage
    {
        public S127DataPackage()
        {
            Id = Guid.NewGuid();
        }
    }
}
