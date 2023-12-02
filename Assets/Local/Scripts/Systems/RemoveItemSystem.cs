using Scripts.Enums;

namespace Scripts
{
    public class RemoveItemSystem: ISystem
    {
        public UIView UIView;
        public PoolCollection<IconView> IconViewPools;

        public void EventCatch(RemoveItemEvent newEvent)
        {
            newEvent.Unit.RemoveItem(newEvent.ItemType, newEvent.Count);

            if (newEvent.ItemType == ItemType.GOLD)
            {
                if (newEvent.Unit is Character && ((Character) newEvent.Unit).CharacterType == CharacterType.PLAYER)
                {
                    var icon = IconViewPools.Get(0);
                    UIView.FlyCoin(icon, false);
                }
                else if (newEvent.Unit is Building)
                {
                    newEvent.Unit.ItemStackView.ToggleExclusiveItemStack(newEvent.ItemType, newEvent.Unit.Items.GetAmount(newEvent.ItemType) > 0);
                }
            }
            else
            {
                newEvent.Unit.ItemStackView.RemoveItem(newEvent.ItemType, newEvent.Count);
            }
        }
    }
}
