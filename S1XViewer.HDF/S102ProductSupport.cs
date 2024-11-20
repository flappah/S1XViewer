using HDF.PInvoke;
using HDF5CSharp.DataTypes;
using PureHDF;
using S1XViewer.HDF.Interfaces;
using System.Runtime.InteropServices;
using System.Text;

namespace S1XViewer.HDF
{
    public class S102ProductSupport : ProductSupportBase, IS102ProductSupport
    {
        public override Hdf5Element? FindFeatureByDateTime(List<Hdf5Element> featureInstances, DateTime? selectedDateTime)
        {
            throw new NotImplementedException();
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
            var groupId = H5G.open(fileId, Encoding.ASCII.GetBytes("/BathymetryCoverage"), H5P.DEFAULT);
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
        ///     Read string attribute productSpecification
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override string GetProductVersion(string fileName)
        {
            using (var file = H5File.OpenRead(fileName))
            {
                var group = file.Group("/");
                var attribute = group.Attribute("productSpecification");
                var data = attribute.ReadString();
                var version = data.First().Replace("INT.IHO.S-102.", "");
                return version;
            }
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
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<(DateTime start, DateTime end)> RetrieveTimeFrameFromHdfDatasetAsync(string hdf5FileName)
        {
            return (DateTime.MinValue, DateTime.MinValue);
        }
    }
}
