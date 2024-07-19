namespace Runtime.Save.Provider
{
    public interface IProvider
    {
        public void Save(string key, string data);
        public string Load(string key);
        public void Delete(string key);
    }
}