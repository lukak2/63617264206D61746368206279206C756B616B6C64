namespace Runtime.Save.Serializer
{
    public interface ISerializer
    {
        public string Serialize<T>(T obj);
        public T Deserialize<T>(string data);
    }
}