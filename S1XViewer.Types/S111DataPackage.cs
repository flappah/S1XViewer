using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S111DataPackage : HdfDataPackage, IS111DataPackage
    {
        public S111DataPackage()
        {
            Id = Guid.NewGuid();
        }
    }
}
