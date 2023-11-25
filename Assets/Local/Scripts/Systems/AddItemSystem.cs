using Scripts.Enums;

namespace Scripts.Systems
{
    public class AddItemSystem: ISystem
    {
        public PoolCollection<ItemView> ItemViewPools;

        public void EventCatch(AddItemEvent newEvent)
        {            
            newEvent.Unit.AddItem(newEvent.ItemType, newEvent.Count);

            if (newEvent.ItemType.Equals(ItemType.GOLD) || newEvent.ItemType.Equals(ItemType.HONOR))
            {
                return;
            }
            
            for (var i = 0; i < newEvent.Count; i++)
            {
                var itemView = ItemViewPools.Get(newEvent.ItemType); 
                newEvent.Unit.ItemStackView.AddItem(itemView);
            }
        }
    }
}
