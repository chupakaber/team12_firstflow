using Scripts.Enums;

namespace Scripts
{
    public class AddItemEvent: IEvent
    {
        public ItemType ItemType;
        public int Count;
        public Unit Unit;
    }
}
