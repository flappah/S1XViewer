using HDF.PInvoke;
using HDF5CSharp.DataTypes;
using S1XViewer.HDF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.HDF
{
    public class S104ProductSupport : ProductSupportBase, IS104ProductSupport
    {
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
    }
}
