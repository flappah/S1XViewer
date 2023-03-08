using S1XViewer.HDF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.HDF
{
    public class ProductSupportFactory : IProductSupportFactory
    {
        public IProductSupportBase[] Supports { get; set; }

        /// <summary>
        ///     Retrieves the ProductSupportFactory specific for the specified product standard
        /// </summary>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public IProductSupportBase Create(string productStandard)
        {
            IProductSupportBase locatedDataParser =
                Supports.ToList().Find(tp => tp.GetType().Name.Contains(productStandard + "ProductSupport"));

            if (locatedDataParser != null)
            {
                return locatedDataParser;
            }

            return new NullProductSupport();
        }
    }
}
