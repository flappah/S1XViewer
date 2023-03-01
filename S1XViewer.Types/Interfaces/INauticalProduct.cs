namespace S1XViewer.Types.Interfaces
{
    public interface INauticalProduct : ICatalogueElements
    {
        IProductSpecification ProductSpecification { get; set; }
        IOnlineResource OnlineResource { get; set; }
        // TextContent is defined in GeoFeatureBase as an array. NauticalProducts uses only element 0!
        IServiceSpecification ServiceSpecification { get; set; }
        string PublicationNumber { get; set; }
        string DataSetName { get; set; }
        string Version { get; set; }
        string ServiceStatus { get; set; }
        string Keyword { get; set; }
        string ServiceDesign { get; set; }
        string ISBN { get; set; }
        string TypeOfProductFormat { get; set; }
    }
}