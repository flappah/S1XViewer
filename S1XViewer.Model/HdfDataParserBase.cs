using HDF5CSharp.DataTypes;
using Microsoft.Isam.Esent.Interop;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.Model.Interfaces;
using System.Globalization;

namespace S1XViewer.Model
{
    public abstract class HdfDataParserBase : DataParserBase, IHdfDataParserBase
    {
        protected static Dictionary<string, Hdf5Element> _cachedHdfTrees = new Dictionary<string, Hdf5Element>();

        /// <summary>
        ///     Async version of hdf5 file retrieval
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        public async Task<Hdf5Element> RetrieveHdf5FileAsync(string hdf5FileName)
        {
            Hdf5Element hdf5S111Root;
            if (_cachedHdfTrees.ContainsKey(hdf5FileName))
            {
                hdf5S111Root = _cachedHdfTrees[hdf5FileName];
            }
            else
            {
                hdf5S111Root = await Task.Factory.StartNew((name) =>
                {
                    // load HDF file, spawned in a seperate task to keep UI responsive!
                    return HDF5CSharp.Hdf5.ReadTreeFileStructure(name.ToString());

                }, hdf5FileName).ConfigureAwait(false);

                _cachedHdfTrees.Add(hdf5FileName, hdf5S111Root);
            }

            return hdf5S111Root;
        }

        /// <summary>
        ///     Sync version of hdf5 file retrieval
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        public Hdf5Element RetrieveHdf5File(string hdf5FileName)
        {
            Hdf5Element hdf5S111Root;
            if (_cachedHdfTrees.ContainsKey(hdf5FileName))
            {
                return hdf5S111Root = _cachedHdfTrees[hdf5FileName];
            }
            else
            {
                return HDF5CSharp.Hdf5.ReadTreeFileStructure(hdf5FileName.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hdf5FileName"></param>
        /// <returns></returns>
        public async Task<(DateTime start, DateTime end)> RetrieveTimeFrameFromHdfDatasetAsync(string hdf5FileName)
        {
            Hdf5Element hdf5S111Root = await RetrieveHdf5FileAsync(hdf5FileName).ConfigureAwait(false);
            if (hdf5S111Root == null || hdf5S111Root.Children.Count == 0 || hdf5S111Root.Children[1].Children.Count == 0)
            {
                return (DateTime.MinValue, DateTime.MinValue);
            }

            DateTime min = DateTime.MaxValue;
            DateTime max = DateTime.MinValue;
            foreach (var surfaceFeature in hdf5S111Root.Children[1].Children)
            {
                if (surfaceFeature != null)
                {
                    var dateTimeOfFirstRecordAttribute = surfaceFeature.Attributes.Find("dateTimeOfFirstRecord");
                    string dateTimeOfFirstRecordString = dateTimeOfFirstRecordAttribute?.Value<string>("") ?? "";

                    DateTime? dateTimeOfFirstRecord = null;
                    if (dateTimeOfFirstRecordString != string.Empty)
                    {
                        dateTimeOfFirstRecord =
                            DateTime.ParseExact(dateTimeOfFirstRecordString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
                    }

                    var dateTimeOfLastRecordAttribute = surfaceFeature.Attributes.Find("dateTimeOfLastRecord");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featuresInstances"></param>
        /// <param name="selectedDateTime"></param>
        /// <returns></returns>
        protected Hdf5Element? FindGroupByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime)
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
            foreach (Hdf5Element? surfaceFeatureInstance in featureInstances)
            {
                if (surfaceFeatureInstance != null)
                {
                    foreach(Hdf5Element? group in surfaceFeatureInstance.Children)
                    {
                        if (group != null)
                        {
                            var timePointttribute = group.Attributes.Find("timePoint");
                            string timePointttributeString = timePointttribute?.Value<string>("") ?? "";

                            DateTime? timePointRecord = null;
                            if (timePointttributeString != string.Empty)
                            {
                                timePointRecord =
                                    DateTime.ParseExact(timePointttributeString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
                            }

                            if (timePointRecord != null)
                            {
                                TimeSpan timespan = ((TimeSpan)(selectedDateTime - timePointRecord)).Duration();
                                if (minimalTimeSpan == null)
                                {
                                    minimalTimeSpan = timespan;
                                    minimalHdf5Element = group;
                                }
                                else
                                {
                                    if (timespan < minimalTimeSpan)
                                    {
                                        minimalTimeSpan = timespan;
                                        minimalHdf5Element = group;
                                    }
                                }
                            }
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

        /// <summary>
        ///     Finds the relevant feature defined in the HDF file by datetime
        /// </summary>
        /// <param name="featureInstances">Feature collection to search through</param>
        /// <param name="selectedDateTime">datetime</param>
        /// <returns>Hdf5Element</returns>
        protected Hdf5Element? FindFeatureByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime)
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
            foreach (Hdf5Element? surfaceFeatureInstance in featureInstances)
            {
                if (surfaceFeatureInstance != null)
                {
                    var dateTimeOfFirstRecordAttribute = surfaceFeatureInstance.Attributes.Find("dateTimeOfFirstRecord");
                    string dateTimeOfFirstRecordString = dateTimeOfFirstRecordAttribute?.Value<string>("") ?? "";

                    DateTime? dateTimeOfFirstRecord = null;
                    if (dateTimeOfFirstRecordString != string.Empty)
                    {
                        dateTimeOfFirstRecord =
                            DateTime.ParseExact(dateTimeOfFirstRecordString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
                    }

                    var dateTimeOfLastRecordAttribute = surfaceFeatureInstance.Attributes.Find("dateTimeOfLastRecord");
                    string dateTimeOfLastRecordString = dateTimeOfLastRecordAttribute?.Value<string>(string.Empty) ?? string.Empty;

                    DateTime? dateTimeOfLastRecord = null;
                    if (dateTimeOfLastRecordString != string.Empty)
                    {
                        dateTimeOfLastRecord =
                            DateTime.ParseExact(dateTimeOfLastRecordString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
                    }

                    if (dateTimeOfFirstRecord != null && dateTimeOfLastRecord != null)
                    {
                        if (selectedDateTime > dateTimeOfFirstRecord && selectedDateTime < dateTimeOfLastRecord)
                        {
                            return surfaceFeatureInstance;
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
                            minimalHdf5Element = surfaceFeatureInstance;
                        }
                        else if (timespanFirst < minimalTimeSpan)
                        {
                            minimalTimeSpan = timespanFirst;
                            minimalHdf5Element = surfaceFeatureInstance;
                        }
                        else if (timespanLast < minimalTimeSpan)
                        {
                            minimalTimeSpan = timespanLast;
                            minimalHdf5Element = surfaceFeatureInstance;
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
    }
}
