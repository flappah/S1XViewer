using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S128DataPackage : XmlDataPackage, IS128DataPackage
    {
        public S128DataPackage()
        {
            Id = Guid.NewGuid();
        }
    }
}
