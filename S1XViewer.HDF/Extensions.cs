using HDF5CSharp.DataTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.HDF
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool Contains(this List<Hdf5AttributeElement> elements, string label)
        {
            foreach(var element in elements)
            {
                if (element.Name == label) return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static Hdf5AttributeElement? Find(this List<Hdf5AttributeElement> elements, string label)
        {
            foreach (var element in elements)
            {
                if (element.Name == label) return element;
            }

            return null;
        }
    }
}
