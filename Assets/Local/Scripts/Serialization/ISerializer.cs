namespace Scripts.Serialization
{
    public interface ISerializer
    {
        void Pack(ByteBuffer buffer);
        void Unpack(ByteBuffer buffer);
    }
}
