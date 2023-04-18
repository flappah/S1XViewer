namespace S1XViewer.Storage.Interfaces
{
    public interface IStateStorage
    {
        string Retrieve(string key);
        bool Store(string key, string value);
    }
}