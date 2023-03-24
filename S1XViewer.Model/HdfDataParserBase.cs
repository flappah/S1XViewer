using HDF5CSharp.DataTypes;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.HDF.Interfaces;
using S1XViewer.Model.Interfaces;
using System.Reflection;

namespace S1XViewer.Model
{
    public abstract class HdfDataParserBase : DataParserBase, IHdfDataParserBase
    {
        protected IProductSupportBase _productSupport;

        /// <summary>
        ///     Retrieves the horizontal CRS
        /// </summary>
        /// <param name="hdf5S111Root"></param>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        protected long RetrieveHorizontalCRS(Hdf5Element hdf5S111Root, string hdf5FileName)
        {
            long horizontalCRS;
            var horizontalCRSAttribute = hdf5S111Root.Attributes.Find("horizontalCRS"); // S111 v1.2
            if (horizontalCRSAttribute != null)
            {
                horizontalCRS = horizontalCRSAttribute?.Value<long>() ?? -1;
                if (horizontalCRS == -1)
                {
                    var nameOfHorizontalCRSAttribute = hdf5S111Root.Attributes.Find("nameOfHorizontalCRS");
                    var nameOfHorizontalCRS = nameOfHorizontalCRSAttribute?.Value<string>() ?? string.Empty;

                    short typeOfHorizontalCRS = _productSupport.GetTypeOfHorizontalCRS(hdf5FileName);
                    if (typeOfHorizontalCRS == 1 || typeOfHorizontalCRS == 2)
                    {
                        var horizontalCSAttribute = hdf5S111Root.Attributes.Find("horizontalCS");
                        var horizontalCS = horizontalCSAttribute?.Value<int>() ?? -1;

                        if (horizontalCS != -1 && horizontalCS.In(6422, 4400, 4500))
                        {
                            horizontalCRS = horizontalCS;
                        }
                        else
                        {
                            var horizontalDatumAttribute = hdf5S111Root.Attributes.Find("horizontalDatum");
                            int horizontalDatum = horizontalDatumAttribute?.Value<int>() ?? -1;

                            if (horizontalDatum != -1)
                            {
                                horizontalCRS = horizontalDatum;
                            }
                        }
                    }
                }
            }
            else
            {
                var horizontalDatumValueAttribute = hdf5S111Root.Attributes.Find("horizontalDatumValue"); // S111 v1.1
                var horizontalDatumValue = horizontalDatumValueAttribute?.Value<int>() ?? 0;

                if (horizontalDatumValue > 0)
                {
                    var epochAttribute = hdf5S111Root.Attributes.Find("epoch");
                    var epoch = epochAttribute?.Value<string>() ?? string.Empty;
                    horizontalCRS = CorrectCRSWithEpoch(horizontalDatumValue, epoch);
                }
                else
                {
                    horizontalCRS = 4326; //base WGS84
                }
            }

            return horizontalCRS;
        }

        /// <summary>
        ///     Retrieves the corrected CRS for baseId and epoch. File crs.csv contains all CRS'ses
        /// </summary>
        /// <param name="baseCRSId"></param>
        /// <param name="epoch"></param>
        /// <returns></returns>
        public long CorrectCRSWithEpoch(long baseCRSId, string epoch)
        {
            if (String.IsNullOrEmpty(epoch) == false)
            {
                var assemblyPath = Assembly.GetAssembly(GetType()).Location;
                string folder = Path.GetDirectoryName(assemblyPath);

                try
                {
                    string[] allCrs = File.ReadAllLines($@"{folder}\crs.csv");
                    List<string> epochLines = allCrs.ToList().FindAll(t => t.Contains($"({epoch.ToUpper()})"));
                    if (epochLines != null && epochLines.Count > 0)
                    {
                        string epochLine = epochLines.Last();
                        var splittedEpochLine = epochLine.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        if (splittedEpochLine.Length == 2)
                        {
                            if (int.TryParse(splittedEpochLine[0], out int epochId))
                            {
                                return epochId;
                            }
                        }
                    }
                }
                catch(Exception ex) { }
            }

            return 4326; // base WGS84
        }

    }
}
