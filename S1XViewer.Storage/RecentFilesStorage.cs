using Microsoft.Isam.Esent.Collections.Generic;
using S1XViewer.Storage.Interfaces;

namespace S1XViewer.Storage
{
    public class RecentFilesStorage : StorageBase, IRecentFilesStorage
    {
        /// <summary>
        /// 
        /// </summary>
        public RecentFilesStorage()
        {
            if (_persistentDictionary == null)
            {
                _persistentDictionary = new PersistentDictionary<string, string>(System.IO.Path.GetTempPath() + @"\S1XViewer\RecentFilesStorage");
            }
        }

        /// <summary>
        ///     Stores the given key, value to the persistent data store
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public override bool Store(string key, string value)
        {
            if (_persistentDictionary.ContainsKey(key))
            {
                _persistentDictionary.Remove(key);
            }

            _persistentDictionary.Add(key, value);
            return true;
        }

        /// <summary>
        ///     Retrieves the value associated with the specified key from the persistent store
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>tuple containing both the status and the result</returns>
        public override string Retrieve(string key)
        {
            if (_persistentDictionary.ContainsKey(key))
            {
                return _persistentDictionary[key];
            }

            return "";
        }
    }
}