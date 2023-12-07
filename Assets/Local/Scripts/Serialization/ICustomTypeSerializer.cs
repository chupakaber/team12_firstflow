namespace Scripts.Serialization
{
    public interface ICustomTypeSerializer<T> where T : new()
    {
        void Put(ByteBuffer buffer, T value);
        T Get(ByteBuffer buffer);
    }
}
