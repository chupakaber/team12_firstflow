namespace Scripts.Systems
{
    public class AddItemSystem: ISystem
    {
        public PoolCollection<ItemView> ItemViewPools;

        public void EventCatch(AddItemEvent newEvent)
        {
            newEvent.Unit.AddItem(newEvent.ItemType, newEvent.Count);
            
            for (var i = 0; i < newEvent.Count; i++)
            {
                var itemView = ItemViewPools.Get(newEvent.ItemType); 
                if (itemView != null)
                {
                    newEvent.Unit.ItemStackView.AddItem(itemView);
                }
            }
        }
    }
}
