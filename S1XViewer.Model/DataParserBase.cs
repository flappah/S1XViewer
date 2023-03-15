using Esri.ArcGISRuntime.Geometry;
using Microsoft.Isam.Esent.Interop;
using S1XViewer.Model.Interfaces;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Model
{
    public abstract class DataParserBase : IDataParser
    {
        public abstract event IDataParser.ProgressFunction? Progress;
       
        /// <summary>
        ///     Parses specified XMLDocument. Async version
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>
        public abstract Task<IS1xxDataPackage> ParseAsync(XmlDocument xmlDocument);

        /// <summary>
        ///     Parses specified HDF5 file. Async version
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        public abstract Task<IS1xxDataPackage> ParseAsync(string hdf5FileName, DateTime? selectedDateTime);

        /// <summary>
        /// Parses specified XMLDocument. 
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>IS1xxDataPackage</returns>

        public abstract IS1xxDataPackage Parse(XmlDocument xmlDocument);

        /// <summary>
        ///     Parses specified HDF5 file
        /// </summary>
        /// <param name="hdf5FileName">HDF5 file name</param>
        /// <param name="selectedDateTime">selected datetime to render data on</param>
        /// <returns>IS1xxDataPackage</returns>
        public abstract IS1xxDataPackage Parse(string hdf5FileName, DateTime? selectedDateTime);

        /// <summary>
        /// Resolves specified links by looking in the specified lists for the requested ID's
        /// </summary>
        /// <param name="links">ILink[]</param>
        /// <param name="informationFeatures">List<IInformationFeature></param>
        /// <param name="metaFeatures">List<IMetaFeature></param>
        /// <param name="geoFeatures">List<IGeoFeature></param>
        protected void ResolveLinks(ILink[] links, List<IInformationFeature> informationFeatures, List<IMetaFeature> metaFeatures, List<IGeoFeature> geoFeatures)
        {
            if (links == null)
            {
                return;
            }

            foreach (ILink link in links)
            {
                int foundInfoFeatureIndex =
                    informationFeatures.FindIndex(ftr =>
                        !String.IsNullOrEmpty(ftr.Id) &&
                        ftr.Id.Contains(link.Href.Replace("#", "")));

                if (foundInfoFeatureIndex != -1)
                {
                    link.Offset = $"I_{foundInfoFeatureIndex}";
                    link.LinkedFeature = informationFeatures[foundInfoFeatureIndex].DeepClone();
                }
                else
                {
                    int foundMetaFeatureIndex =
                        metaFeatures.FindIndex(ftr =>
                            !String.IsNullOrEmpty(ftr.Id) &&
                            ftr.Id.Contains(link.Href.Replace("#", "")));

                    if (foundMetaFeatureIndex != -1)
                    {
                        link.Offset = $"M_{foundMetaFeatureIndex}";
                        link.LinkedFeature = metaFeatures[foundMetaFeatureIndex].DeepClone();
                    }
                    else
                    {
                        int foundGeoFeatureIndex =
                            geoFeatures.FindIndex(ftr =>
                                !String.IsNullOrEmpty(ftr.Id) &&
                                ftr.Id.Contains(link.Href.Replace("#", "")));

                        if (foundGeoFeatureIndex != -1)
                        {
                            link.Offset = $"G_{foundGeoFeatureIndex}";
                            link.LinkedFeature = geoFeatures[foundGeoFeatureIndex].DeepClone();
                        }
                    }
                }
            }
        }
    }
}
