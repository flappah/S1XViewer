using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IPhysicalProduct : INavigationalProduct
    {
        DateTime EditionDate { get; set; }
        string ISBN { get; set; }
        IPrintInformation PrintInformation { get; set; }
        string PublicationNumber { get; set; }
        DateTime ReferenceToNM { get; set; }
        string TypeOfPaper { get; set; }
    }
}