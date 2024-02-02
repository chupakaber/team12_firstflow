using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class ItemView: PoolableObject
    {
        public ItemType ItemType;
        public Vector3 LastPosition;
        public Vector3 CurrentPosition;
    }
}
