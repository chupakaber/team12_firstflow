using System;
using System.Text;

namespace Scripts.Serialization
{
    public class SerializationUtils
    {
        public const int SIZE_OF_BOOL = sizeof(bool);
        public const int SIZE_OF_SHORT = sizeof(short);
        public const int SIZE_OF_INT = sizeof(int);
        public const int SIZE_OF_LONG = sizeof(long);
        public const int SIZE_OF_FLOAT = sizeof(float);

        public static void Put(ByteBuffer buffer, bool value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(buffer.Data, buffer.Pointer);
            buffer.Pointer += bytes.Length;
        }

        public static void Put(ByteBuffer buffer, short value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(buffer.Data, buffer.Pointer);
            buffer.Pointer += bytes.Length;
        }

        public static void Put(ByteBuffer buffer, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(buffer.Data, buffer.Pointer);
            buffer.Pointer += bytes.Length;
        }

        public static void Put(ByteBuffer buffer, long value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(buffer.Data, buffer.Pointer);
            buffer.Pointer += bytes.Length;
        }

        public static void Put(ByteBuffer buffer, float value)
        {
            var bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(buffer.Data, buffer.Pointer);
            buffer.Pointer += bytes.Length;
        }

        public static void Put(ByteBuffer buffer, string value, bool isUTF8 = true)
        {
            Put(buffer, (short) (isUTF8 ? 1 : 0));
            var bytes = (isUTF8 ? Encoding.UTF8 : Encoding.ASCII).GetBytes(value);
            Put(buffer, bytes.Length);
            bytes.CopyTo(buffer.Data, buffer.Pointer);
            buffer.Pointer += bytes.Length;
        }

        public static void Put<T>(ByteBuffer buffer, ICustomTypeSerializer<T> serializer, T value) where T : new()
        {
            serializer.Put(buffer, value);
        }

        public static bool Get(ByteBuffer buffer, bool defaultValue = false)
        {
            if (buffer.Data.Length >= buffer.Pointer + SIZE_OF_BOOL)
            {
                var value = BitConverter.ToBoolean(buffer.Data, buffer.Pointer);
                buffer.Pointer += SIZE_OF_BOOL;
                return value;
            }
            return defaultValue;
        }

        public static short Get(ByteBuffer buffer, short defaultValue = -1)
        {
            if (buffer.Data.Length >= buffer.Pointer + SIZE_OF_SHORT)
            {
                var value = BitConverter.ToInt16(buffer.Data, buffer.Pointer);
                buffer.Pointer += SIZE_OF_SHORT;
                return value;
            }
            return defaultValue;
        }

        public static int Get(ByteBuffer buffer, int defaultValue = -1)
        {
            if (buffer.Data.Length >= buffer.Pointer + SIZE_OF_INT)
            {
                var value = BitConverter.ToInt32(buffer.Data, buffer.Pointer);
                buffer.Pointer += SIZE_OF_INT;
                return value;
            }
            return defaultValue;
        }

        public static long Get(ByteBuffer buffer, long defaultValue = -1)
        {
            if (buffer.Data.Length >= buffer.Pointer + SIZE_OF_LONG)
            {
                var value = BitConverter.ToInt64(buffer.Data, buffer.Pointer);
                buffer.Pointer += SIZE_OF_LONG;
                return value;
            }
            return defaultValue;
        }

        public static float Get(ByteBuffer buffer, float defaultValue = -1f)
        {
            if (buffer.Data.Length >= buffer.Pointer + SIZE_OF_FLOAT)
            {
                var value = BitConverter.ToSingle(buffer.Data, buffer.Pointer);
                buffer.Pointer += SIZE_OF_FLOAT;
                return value;
            }
            return defaultValue;
        }

        public static string Get(ByteBuffer buffer, string defaultValue = "")
        {
            var encoding = Get(buffer, (short) 0);
            var length = Get(buffer, (int) 0);
            if (buffer.Data.Length >= buffer.Pointer + length)
            {
                var value = (encoding == 1 ? Encoding.UTF8 : Encoding.ASCII).GetString(buffer.Data, buffer.Pointer, length);
                buffer.Pointer += length;
                return value;
            }
            return defaultValue;
        }

        public static T Get<T>(ByteBuffer buffer, ICustomTypeSerializer<T> serializer) where T : new()
        {
            return serializer.Get(buffer);
        }

        public static int GetStringLength(string value, bool isUTF8)
        {
            return (isUTF8 ? Encoding.UTF8 : Encoding.ASCII).GetByteCount(value) + SIZE_OF_SHORT + SIZE_OF_INT;
        }
    }
}
