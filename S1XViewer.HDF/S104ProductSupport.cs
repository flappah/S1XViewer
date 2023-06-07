using HDF.PInvoke;
using HDF5CSharp.DataTypes;
using S1XViewer.HDF.Interfaces;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace S1XViewer.HDF
{
    public class S104ProductSupport : ProductSupportBase, IS104ProductSupport
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureInstances"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override Hdf5Element? FindFeatureByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime)
        {
            if (featureInstances is null)
            {
                throw new ArgumentNullException(nameof(featureInstances));
            }

            if (selectedDateTime is null)
            {
                throw new ArgumentNullException(nameof(selectedDateTime));
            }

            TimeSpan? minimalTimeSpan = null;
            Hdf5Element? minimalHdf5Element = null;
            foreach (Hdf5Element? waterlevelFeatureInstance in featureInstances)
            {
                if (waterlevelFeatureInstance != null)
                {
                    var dateTimeOfFirstRecordAttribute = waterlevelFeatureInstance.Attributes.Find("dateTimeOfFirstRecord");
                    string dateTimeOfFirstRecordString = dateTimeOfFirstRecordAttribute?.Value<string>("") ?? "";

                    DateTime? dateTimeOfFirstRecord = null;
                    if (dateTimeOfFirstRecordString != string.Empty)
                    {
                        dateTimeOfFirstRecord =
                            DateTime.ParseExact(dateTimeOfFirstRecordString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
                    }

                    var dateTimeOfLastRecordAttribute = waterlevelFeatureInstance.Attributes.Find("dateTimeOfLastRecord");
                    string dateTimeOfLastRecordString = dateTimeOfLastRecordAttribute?.Value<string>(string.Empty) ?? string.Empty;

                    DateTime? dateTimeOfLastRecord = null;
                    if (dateTimeOfLastRecordString != string.Empty)
                    {
                        dateTimeOfLastRecord =
                            DateTime.ParseExact(dateTimeOfLastRecordString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
                    }

                    if (dateTimeOfFirstRecord != null && dateTimeOfLastRecord != null)
                    {
                        if (selectedDateTime >= dateTimeOfFirstRecord && selectedDateTime <= dateTimeOfLastRecord)
                        {
                            return waterlevelFeatureInstance;
                        }

                        TimeSpan timespanFirst = ((TimeSpan)(selectedDateTime - dateTimeOfFirstRecord)).Duration();
                        TimeSpan timespanLast = ((TimeSpan)(selectedDateTime - dateTimeOfLastRecord)).Duration();

                        if (minimalTimeSpan == null)
                        {
                            if (timespanFirst < timespanLast)
                            {
                                minimalTimeSpan = timespanFirst;
                            }
                            else
                            {
                                minimalTimeSpan = timespanLast;
                            }
                            minimalHdf5Element = waterlevelFeatureInstance;
                        }
                        else if (timespanFirst < minimalTimeSpan)
                        {
                            minimalTimeSpan = timespanFirst;
                            minimalHdf5Element = waterlevelFeatureInstance;
                        }
                        else if (timespanLast < minimalTimeSpan)
                        {
                            minimalTimeSpan = timespanLast;
                            minimalHdf5Element = waterlevelFeatureInstance;
                        }
                    }
                }
            }

            if (minimalTimeSpan != null)
            {
                return minimalHdf5Element;
            }

            return null;
        }

        public override Hdf5Element? FindGroupByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Retrieves the dataCodingFormat
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override short GetDataCodingFormat(string fileName)
        {
            var fileId = H5F.open(fileName, H5F.ACC_RDONLY, H5P.DEFAULT);
            var groupId = H5G.open(fileId, Encoding.ASCII.GetBytes("/WaterLevel"), H5P.DEFAULT);
            var attribId = H5A.open(groupId, Encoding.ASCII.GetBytes("dataCodingFormat"), H5P.DEFAULT);

            var value = new byte[2];
            var handle = GCHandle.Alloc(value, GCHandleType.Pinned);
            try
            {
                if (H5A.read(attribId, H5T.NATIVE_UINT8, handle.AddrOfPinnedObject()) == 0)
                {
                    var returnValue = BitConverter.ToInt16(value, 0);
                    return returnValue;
                }
            }
            finally
            {
                handle.Free();
                H5A.close(attribId);
                H5G.close(groupId);
                H5F.close(fileId);
            }

            return -1;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override short GetTypeOfHorizontalCRS(string fileName)
        {
            var fileId = H5F.open(fileName, H5F.ACC_RDONLY, H5P.DEFAULT);
            var groupId = H5G.open(fileId, Encoding.ASCII.GetBytes("/"), H5P.DEFAULT);
            var attribId = H5A.open(groupId, Encoding.ASCII.GetBytes("typeOfHorizontalCRS"), H5P.DEFAULT);

            var value = new byte[2];
            var handle = GCHandle.Alloc(value, GCHandleType.Pinned);
            try
            {
                if (H5A.read(attribId, H5T.NATIVE_UINT8, handle.AddrOfPinnedObject()) == 0)
                {
                    var returnValue = BitConverter.ToInt16(value, 0);
                    return returnValue;
                }
            }
            finally
            {
                handle.Free();
                H5A.close(attribId);
                H5G.close(groupId);
                H5F.close(fileId);
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        public override async Task<(DateTime start, DateTime end)> RetrieveTimeFrameFromHdfDatasetAsync(string hdf5FileName)
        {
            Hdf5Element hdf5S111Root = await RetrieveHdf5FileAsync(hdf5FileName).ConfigureAwait(false);
            if (hdf5S111Root == null || hdf5S111Root.Children.Count == 0 || hdf5S111Root.Children[1].Children.Count == 0)
            {
                return (DateTime.MinValue, DateTime.MinValue);
            }

            DateTime min = DateTime.MaxValue;
            DateTime max = DateTime.MinValue;
            foreach (var waterLevelFeature in hdf5S111Root.Children[1].Children)
            {
                if (waterLevelFeature != null)
                {
                    var dateTimeOfFirstRecordAttribute = waterLevelFeature.Attributes.Find("dateTimeOfFirstRecord");
                    string dateTimeOfFirstRecordString = dateTimeOfFirstRecordAttribute?.Value<string>("") ?? "";

                    DateTime? dateTimeOfFirstRecord = null;
                    if (dateTimeOfFirstRecordString != string.Empty)
                    {
                        dateTimeOfFirstRecord =
                            DateTime.ParseExact(dateTimeOfFirstRecordString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
                    }

                    var dateTimeOfLastRecordAttribute = waterLevelFeature.Attributes.Find("dateTimeOfLastRecord");
                    string dateTimeOfLastRecordString = dateTimeOfLastRecordAttribute?.Value<string>(string.Empty) ?? string.Empty;

                    DateTime? dateTimeOfLastRecord = null;
                    if (dateTimeOfLastRecordString != string.Empty)
                    {
                        dateTimeOfLastRecord =
                            DateTime.ParseExact(dateTimeOfLastRecordString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
                    }

                    if (dateTimeOfFirstRecord < min)
                    {
                        min = (DateTime)dateTimeOfFirstRecord;
                    }

                    if (dateTimeOfLastRecord > max)
                    {
                        max = (DateTime)dateTimeOfLastRecord;
                    }
                }
            }

            if (min != DateTime.MaxValue && max != DateTime.MinValue)
            {
                return (min, max);
            }


            return (DateTime.MinValue, DateTime.MinValue);
        }
    }
}
