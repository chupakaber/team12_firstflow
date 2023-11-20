using Scripts.Enums;

namespace Scripts.Systems
{
    public class CraftSystem: ISystem
    {
        public void EventCatch(AddItemEvent newEvent)
        {            
            newEvent.Character.AddItem(newEvent.ItemType, newEvent.Count);            
        }
    }
}
