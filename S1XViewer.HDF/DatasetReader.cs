using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HDF5CSharp;
using S1XViewer.HDF.Interfaces;

namespace S1XViewer.HDF
{
    public class DatasetReader : IDatasetReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<T> Read<T>(string fileName, string name)
        {
            var fileId = Hdf5.OpenFile(fileName);

            try
            {
                var values = Hdf5.ReadCompounds<T>(fileId, name, "", false);
                return values;
            }
            catch { }
            finally
            {
                Hdf5.CloseFile(fileId);
            }

            return default(IEnumerable<T>);
        }
    }
}
