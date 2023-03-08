﻿using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using HDF5CSharp.DataTypes;
using S1XViewer.Base;
using S1XViewer.HDF;
using S1XViewer.Model.Interfaces;
using System;
using System.Globalization;

namespace S1XViewer.Model
{
    public abstract class HdfDataParserBase : DataParserBase, IHdfDataParserBase
    {
        protected static Dictionary<string, Hdf5Element> _cachedHdfTrees = new Dictionary<string, Hdf5Element>();

        /// <summary>
        ///     Finds the relevant feature defined in the HDF file by datetime
        /// </summary>
        /// <param name="features">Feature collection to search through</param>
        /// <param name="selectedDateTime">datetime</param>
        /// <returns>Hdf5Element</returns>
        protected Hdf5Element? FindFeatureByDateTime(List<Hdf5Element> features, DateTime? selectedDateTime)
        {
            if (features is null)
            {
                throw new ArgumentNullException(nameof(features));
            }

            if (selectedDateTime is null)
            {
                throw new ArgumentNullException(nameof(selectedDateTime));
            }

            var minimalDateTime = DateTime.MaxValue;
            var maximalDateTime = DateTime.MinValue;
            Hdf5Element? minimalHdf5Element = null;
            Hdf5Element? maximalHdf5Element = null;
            foreach (Hdf5Element? hdf5Element in features)
            {
                if (hdf5Element != null)
                {
                    var dateTimeOfFirstRecordAttribute = hdf5Element.Attributes.Find("dateTimeOfFirstRecord");
                    string dateTimeOfFirstRecordString = dateTimeOfFirstRecordAttribute?.Value<string>("") ?? "";

                    DateTime? dateTimeOfFirstRecord = null;
                    if (dateTimeOfFirstRecordString != string.Empty)
                    {
                        dateTimeOfFirstRecord =
                            DateTime.ParseExact(dateTimeOfFirstRecordString, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
                    }

                    var dateTimeOfLastRecordAttribute = hdf5Element.Attributes.Find("dateTimeOfLastRecord");
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
                            return hdf5Element;
                        }

                        if (minimalDateTime > dateTimeOfFirstRecord)
                        {
                            minimalDateTime = (DateTime) dateTimeOfFirstRecord;
                            minimalHdf5Element = hdf5Element;
                        }

                        if (maximalDateTime < dateTimeOfLastRecord)
                        {
                            maximalDateTime = (DateTime) dateTimeOfLastRecord;
                            maximalHdf5Element = hdf5Element;
                        }
                    }
                }
            }

            if (selectedDateTime < minimalDateTime)
            {
                return minimalHdf5Element;
            }

            if (selectedDateTime > maximalDateTime)
            {
                return maximalHdf5Element;
            }

            return null;
        }

    }
}
