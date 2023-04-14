using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S122DataPackage : XmlDataPackage, IS122DataPackage
    {
        public S122DataPackage()
        {
            Id = Guid.NewGuid();
        }
    }
}
