using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HDF.PInvoke;
using HDF5CSharp;
using PureHDF;
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
            if (fileId != -1)
            {
                try
                {
                    var values = Hdf5.ReadCompounds<T>(fileId, name, "", true);
                    return values;
                }
                catch { }
                finally
                {
                    Hdf5.CloseFile(fileId);
                }
            }
            
            return default(IEnumerable<T>);
        }

        /// <summary>
        ///     to read 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="name"></param>
        /// <param name="dim1"></param>
        /// <param name="dim2"></param>
        /// <returns></returns>
        public float[,] ReadArrayOfFloats(string fileName, string name, int dim1, int dim2)
        {
            var mmf = MemoryMappedFile.CreateFromFile(fileName);
            var accessor = mmf.CreateViewAccessor();
            var file = H5File.Open(accessor);

            if (file != null)
            {
                try
                {
                    var dataset = file.Dataset(name);
                    var data2d = dataset.Read<float>().ToArray2D<float>(dim1, dim2);
                    return data2d;
                }
                catch { }
                finally
                {
                    mmf.Dispose();
                }
            }

            return new float[0, 0]; 
        }
    }
}
