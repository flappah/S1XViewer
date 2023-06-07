using System.Data;
using System.Text.RegularExpressions;
using System.Xml;

namespace S1XViewer.Base
{
    public static class Extensions
    {
        /// <summary>
        ///     Returns true if the specified date is between the start- and end values
        /// </summary>
        /// <param name="item"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool Between(this DateTime item, DateTime start, DateTime end)
        {
            return item >= start && item <= end;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Contains(this XmlAttributeCollection? collection, string name)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (XmlAttribute item in collection)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XmlAttribute Find(this XmlAttributeCollection? collection, string name)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (XmlAttribute item in collection)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        ///      Checks if one or more of the given itemstrings are contained in the target item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="items"></param>
        /// <returns>True if one of the list of string items is contained inside the specified string</returns>
        public static bool Contains(this string item, bool ignoreCase = false, params string[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (string value in items)
            {
                if (ignoreCase)
                {
                    if (item.ToUpper().Contains(value.ToUpper()))
                    {
                        return true;
                    }
                }
                else
                {
                    if (item.Contains(value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static (bool result, object value) ContainsKey(this DataRowCollection item, string fieldName)
        {
            if (item.Count > 0)
            {
                foreach (DataRow row in item)
                {
                    if (row.ItemArray[0].Equals(fieldName) == true)
                    {
                        return (true, row.ItemArray[1] ?? "");
                    }
                }
            }

            return (false, "");
        }

        /// <summary>
        ///     Returns true if the string contains a numerical value
        /// </summary>
        /// <param name="item">string to execute method on</param>
        /// <returns></returns>
        public static bool IsNumeric(this string item)
        {
            return (int.TryParse(item, out _) || double.TryParse(item, out _));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool In<T>(this T item, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Contains(item);
        }

        /// <summary>
        ///     Joins a list of strings to a string separated by the specified delimeter
        /// </summary>
        /// <param name="items">list of strings to join</param>
        /// <param name="delimeter">delimeter. No delimeter means comma</param>
        /// <returns>joined string</returns>
        public static string Join(this List<string> items, string delimeter)
        {
            if (items == null)
                return "";

            if (String.IsNullOrEmpty(delimeter))
            {
                delimeter = ",";
            }

            return String.Join<string>(delimeter, items);
        }

        /// <summary>
        ///    Clone implementation for lists 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this List<T> items) where T : ICloneable
        {
            return items.Select(item => (T)item.Clone()).ToList();
        }

        /// <summary>
        ///     Returns part after specified start character
        /// </summary>
        /// <param name="item">extension string</param>
        /// <param name="startFrom">start from specified character</param>
        /// <returns>New string</returns>
        public static string LastPart(this string item, char startFrom)
        {
            if (item.Length == 0)
            {
                return item;
            }

            var start = item.LastIndexOf(startFrom);
            if (start <= 0)
            {
                return item;
            }

            return item.Substring(start + 1, item.Length - start - 1);
        }

        /// <summary>
        ///     Returns part after specified start character
        /// </summary>
        /// <param name="item">extension string</param>
        /// <param name="startFrom">start from specified character</param>
        /// <returns>New string</returns>
        public static string LastPart(this string item, string startFrom)
        {
            if (item.Length == 0)
            {
                return item;
            }

            var start = item.LastIndexOf(char.Parse(startFrom));
            if (start <= 0)
            {
                return item;
            }

            return item.Substring(start + 1, item.Length - start - 1);
        }

        /// <summary>
        ///     Limits the amount of listitems to the specified count
        /// </summary>
        /// <param name="item">List</param>
        /// <param name="itemAmount">Amount of items to limit to</param>
        /// <returns>Resulting list</returns>
        public static List<T> LimitTo<T>(this List<T> item, int itemAmount)
        {
            if (item == null)
            {
                return null;
            }

            if (item.Count > itemAmount)
            {
                item.RemoveRange(itemAmount, item.Count - itemAmount);
            }

            return item;
        }

        /// <summary>
        ///     Cut length characters off of the left part of the string
        /// </summary>
        /// <param name="item">extension string</param>
        /// <param name="length">number of characters to cut off</param>
        /// <returns>New string</returns>
        public static string StripLeft(this string item, int length)
        {
            return item.Substring(length, item.Length - length);
        }

        /// <summary>
        ///     Cut length characters off of the right part of the string
        /// </summary>
        /// <param name="item">extension string</param>
        /// <param name="length">number of characters to cut off</param>
        /// <returns>New string</returns>
        public static string StripRight(this string item, int length)
        {
            return item.Substring(0, item.Length - length);
        }

        /// <summary>
        /// Converts string to float
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static float ToFloat(this string item)
        {
            if (!float.TryParse(item, out float convertedValue))
            {
                float.TryParse(item.Replace(".", ","), out convertedValue);
            }

            return convertedValue;
        }

        /// <summary>
        /// Converts string to double
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static double ToDouble(this string item)
        {
            if (!double.TryParse(item, out double convertedValue))
            {
                double.TryParse(item.Replace(".", ","), out convertedValue);
            }

            return convertedValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<string> RegexSplit(this String item)
        {
            var splittedList = new List<string>();
            if (!String.IsNullOrEmpty(item))
            {
                if (item.Contains(","))
                {
                    splittedList.AddRange(item.Split(new[] { ',' }));
                }
                else
                {
                    var regex = new Regex("[a-zA-Z0-9$]*");
                    var replacedNotAllowedAcronyms = regex.Replace(item, "");
                    if (replacedNotAllowedAcronyms.Length > 0)
                    {
                        var separationCharacter = replacedNotAllowedAcronyms[0];
                        splittedList.AddRange(item.Split(separationCharacter));
                    }
                }
            }

            return splittedList;
        }

        /// <summary>
        /// Reworked Split implementation that first assumes the comma as it separationchar but if the assumption isn't valid it goes on to 
        /// determine the separationchar using a regex
        /// </summary>
        /// <param name="item">String containing multiple items separated by a specific character</param>
        /// <param name="separationString">New separation STRING used to recombine the stringitems</param>
        /// <returns></returns>
        public static string Recombine(this String item, string separationString = ",")
        {
            var splittedList = item.RegexSplit();

            var recombinedString = "";
            if (splittedList.Count > 0)
            {
                foreach (var acronym in splittedList)
                {
                    recombinedString += $"{(separationString.Length > 1 ? separationString[0].ToString() : "")}{acronym}{separationString}";
                }
                recombinedString = recombinedString.Substring(0, recombinedString.Length - 1);
            }

            return recombinedString;
        }
    }
}
