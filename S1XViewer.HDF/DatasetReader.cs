using HDF5CSharp;
using PureHDF;
using S1XViewer.HDF.Interfaces;
using System.IO.MemoryMappedFiles;
using System.Printing;
using System.Reflection;
using System.Reflection.Metadata;

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
            var fileId = Hdf5.OpenFile(fileName, true);
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
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<string> ReadStrings(string fileName, string name)
        {
            var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open);
            var accessor = mmf.CreateViewAccessor();
            var file = H5File.Open(accessor);

            if (file != null)
            {
                try
                {
                    var dataset = file.Dataset(name);
                    var data = dataset.ReadString();
                    return data == null ? new string[0] : data;
                }
                catch { }
                finally
                {
                    accessor.Dispose();
                    file.Dispose();
                    mmf.Dispose();
                }
            }

            return new string[0];
        }

        /// <summary>
        ///     to read 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public (string[] members, T[] values) ReadCompound<T>(string fileName, string name) where T : struct
        {
            var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open);
            var accessor = mmf.CreateViewAccessor();
            var file = H5File.Open(accessor);

            if (file != null)
            {
                try
                {
                    var dataset = file.Dataset(name);

                    var members = dataset.Type.Compound.Members;
                    var compoundMembers = new List<string>();
                    foreach (var member in members)
                    {
                        compoundMembers.Add(member.Name);
                    }

                    Func<FieldInfo, string> converter = fieldInfo =>
                    {
                        var attribute = fieldInfo.GetCustomAttribute<H5NameAttribute>(true);
                        return attribute is not null ? attribute.Name : fieldInfo.Name;
                    };

                    T[] values = dataset.ReadCompound<T>(converter);                    
                    return (compoundMembers.ToArray(), values);
                }
                catch { }
                finally
                {
                    accessor.Dispose();
                    file.Dispose();
                    mmf.Dispose();
                }
            }

            return (new string[0], new T[0]);
        }

        /// <summary>
        ///     to read 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="name"></param>
        /// <param name="dim1"></param>
        /// <param name="dim2"></param>
        /// <returns></returns>
        public (string[] members, float[,] values) ReadArrayOfFloats(string fileName, string name, int dim1, int dim2)
        {
            var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open);
            var accessor = mmf.CreateViewAccessor();
            var file = H5File.Open(accessor);

            if (file != null)
            {
                try
                {
                    var dataset = file.Dataset(name);

                    var members = dataset.Type.Compound.Members;
                    var compoundMembers = new List<string>();
                    foreach(var member in members ) 
                    {
                        compoundMembers.Add(member.Name);
                    }

                    var data2d = dataset.Read<float>().ToArray2D<float>(dim1, dim2);
                    return (compoundMembers.ToArray(), data2d);
                }
                catch { }
                finally
                {
                    accessor.Dispose();
                    file.Dispose();
                    mmf.Dispose();
                }
            }

            return (new string[0], new float[0, 0]); 
        }
    }
}
