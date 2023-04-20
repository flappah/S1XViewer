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
            foreach (var element in elements)
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Value<T>(this Hdf5AttributeElement element, T defaultValue = default(T))
        {
            if (element != null && element.Values != null)
            {
                var value = ((Array)element.Values).GetValue(0) ?? "";

                if (typeof(T).ToString().ToUpper().Contains("DOUBLE"))
                {
                    if (double.TryParse(value.ToString(), out double doubleValue))
                    {
                        return (T)(object)doubleValue;
                    }
                }
                else if (typeof(T).ToString().ToUpper().Contains("FLOAT"))
                {
                    if (float.TryParse(value.ToString(), out float floatValue))
                    {
                        return (T)(object)floatValue;
                    }
                }
                else if (typeof(T).ToString().ToUpper().Contains("INT64") || typeof(T).ToString().ToUpper().Contains("LONG"))
                {
                    return (T)(object)Convert.ToInt64(value.ToString());
                }
                else if (typeof(T).ToString().ToUpper().Contains("INT32") || typeof(T).ToString().ToUpper().Contains("INT"))
                {
                    return (T)(object)Convert.ToInt32(value.ToString());
                }

                return (T)value;
            }

            return defaultValue;
        }
    }
}

