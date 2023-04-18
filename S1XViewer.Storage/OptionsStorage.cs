using Microsoft.Isam.Esent.Collections.Generic;
using S1XViewer.Storage.Interfaces;
using System.IO;

namespace S1XViewer.Storage
{
    public class OptionsStorage : StorageBase, IOptionsStorage
    {
        /// <summary>
        /// 
        /// </summary>
        public OptionsStorage()
        {
        }

        /// <summary>
        ///     Stores the given key, value to the persistent data store
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public override bool Store(string key, string value)
        {
            using (var persistentDictionary = new PersistentDictionary<string, string>(System.IO.Path.GetTempPath() + @"\S1XViewer\OptionsStorage"))
            {
                if (persistentDictionary.ContainsKey(key))
                {
                    persistentDictionary.Remove(key);
                }

                persistentDictionary.Add(key, value);
                return true;
            }
        }

        /// <summary>
        ///     Retrieves the value associated with the specified key from the persistent store
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>tuple containing both the status and the result</returns>
        public override string Retrieve(string key)
        {
            using (var persistentDictionary = new PersistentDictionary<string, string>(System.IO.Path.GetTempPath() + @"\S1XViewer\OptionsStorage"))
            {
                if (persistentDictionary.ContainsKey(key))
                {
                    return persistentDictionary[key];
                }

                return "";
            }
        }
    }
}
