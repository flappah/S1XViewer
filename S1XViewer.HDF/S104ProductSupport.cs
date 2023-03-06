using HDF5CSharp.DataTypes;
using S1XViewer.HDF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.HDF
{
    public class S104ProductSupport : ProductSupportBase, IS104ProductSupport
    {
        public override int GetDataCodingFormat(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
