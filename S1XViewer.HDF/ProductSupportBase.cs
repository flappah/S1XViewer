using S1XViewer.HDF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.HDF
{
    public abstract class ProductSupportBase : IProductSupportBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public abstract int GetDataCodingFormat(string fileName);
    }
}
