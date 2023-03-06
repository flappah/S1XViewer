using HDF5CSharp.DataTypes;
using S1XViewer.HDF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.HDF
{
    public class NullProductSupport : ProductSupportBase, INullProductSupport
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public override short GetDataCodingFormat(string fileName)
        {
            return -1;
        }
    }
}
