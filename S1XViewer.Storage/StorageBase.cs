using Microsoft.Isam.Esent.Collections.Generic;
using S1XViewer.Storage.Interfaces;

namespace S1XViewer.Storage
{
    public abstract class StorageBase : IStorage
    {
        protected static PersistentDictionary<string, string> _persistentDictionary;

        /// <summary>
        ///     Returns the number of items in the persistent storage
        /// </summary>
        public virtual int Count
        {
            get
            {
                return _persistentDictionary.Count;
            }
        }

        public abstract string Retrieve(string key);
        public abstract bool Store(string key, string value);
    }
}
