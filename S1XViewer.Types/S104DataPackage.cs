using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types
{
    public class S104DataPackage : HdfDataPackage, IS104DataPackage
    {
        public S104DataPackage()
        {
            Id = Guid.NewGuid();
        }
    }
}
