using Scripts.Serialization;
using UnityEngine;

namespace Scripts.Serializators
{
    public class Vector3Serializer<T> : ICustomTypeSerializer<T> where T : new()
    {
        public const int SIZE_OF_STRUCTURE = SerializationUtils.SIZE_OF_FLOAT * 3;

        public static Vector3Serializer<Vector3> Instance { get {
            if (_instance == null)
            {
                _instance = new Vector3Serializer<Vector3>();
            }

            return _instance;
        } }

        private static Vector3Serializer<Vector3> _instance;

        void ICustomTypeSerializer<T>.Put(ByteBuffer buffer, T value)
        {
            if (value is not Vector3)
            {
                throw new System.Exception($"Input value type incorrect. It's must be Vector3, but it is {value?.GetType().Name}");
            }

            var v = (Vector3)(object)value;
            SerializationUtils.Put(buffer, v.x);
            SerializationUtils.Put(buffer, v.y);
            SerializationUtils.Put(buffer, v.z);
        }

        public T Get(ByteBuffer buffer)
        {
            var v = new Vector3();
            v.x = SerializationUtils.Get(buffer, 0f);
            v.y = SerializationUtils.Get(buffer, 0f);
            v.z = SerializationUtils.Get(buffer, 0f);
            return (T) (object) v;
        }
    }
}