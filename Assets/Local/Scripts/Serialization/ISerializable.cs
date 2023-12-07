namespace Scripts.Serialization
{
    public interface ISerializable
    {
        void Pack(ByteBuffer buffer);
        void Unpack(ByteBuffer buffer);
    }
}
