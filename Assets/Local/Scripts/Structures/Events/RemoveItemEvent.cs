using Scripts.Enums;

namespace Scripts
{
    public class RemoveItemEvent : IEvent
    {
        public ItemType ItemType;
        public int Count;
        public Unit Unit;
    }
}
