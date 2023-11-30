using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class AddItemEvent: IEvent
    {
        public ItemType ItemType;
        public int Count;
        public Unit Unit;
        public Vector3 FromPosition;
    }
}
