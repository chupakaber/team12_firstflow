using Scripts.Enums;

namespace Scripts.Systems
{
    public class AddItemSystem: ISystem
    {
        public PoolCollection<ItemView> ItemViewPools;
        public PoolCollection<IconView> IconViewPools;
        public UIView UIView;

        public void EventCatch(AddItemEvent newEvent)
        {
            newEvent.Unit.AddItem(newEvent.ItemType, newEvent.Count);
            
            for (var i = 0; i < newEvent.Count; i++)
            {
                var itemView = ItemViewPools.Get((int) newEvent.ItemType); 
                if (itemView != null)
                {
                    newEvent.Unit.ItemStackView.AddItem(itemView);
                }
                else
                {
                    if (newEvent.ItemType == ItemType.GOLD)
                    {
                        var icon = IconViewPools.Get(0);
                        UIView.FlyIcon(icon);
                    }
                }
            }
        }
    }
}
