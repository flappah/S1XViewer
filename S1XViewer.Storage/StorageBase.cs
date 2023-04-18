using Microsoft.Isam.Esent.Collections.Generic;
using S1XViewer.Storage.Interfaces;

namespace S1XViewer.Storage
{
    public abstract class StorageBase : IStorage
    {
        public abstract string Retrieve(string key);
        public abstract bool Store(string key, string value);
    }
}
