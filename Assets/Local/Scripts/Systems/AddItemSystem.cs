using DG.Tweening;
using Scripts.Enums;
using UnityEngine;

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
                    var endPosition = newEvent.Unit.GetStackTopPosition(newEvent.ItemType);
                    if (newEvent.FromPosition.sqrMagnitude > float.Epsilon)
                    {
                        itemView.gameObject.SetActive(false);
                        var itemViewForFly = ItemViewPools.Get((int) newEvent.ItemType);
                        itemViewForFly.transform.position = newEvent.FromPosition;
                        itemViewForFly.transform.DOJump(endPosition, 1f, 1, 0.5f).OnComplete(() => {
                            itemViewForFly.Release();
                            itemView.gameObject.SetActive(true);
                            newEvent.Unit.ItemStackView.SortItems();
                        });
                    }
                }
                else
                {
                    if (newEvent.ItemType == ItemType.GOLD)
                    {
                        if (newEvent.Unit is Character && ((Character) newEvent.Unit).CharacterType == CharacterType.PLAYER)
                        {
                            var icon = IconViewPools.Get(0);
                            UIView.FlyCoin(icon, true);
                        }
                        else if (newEvent.Unit is Building)
                        {
                            newEvent.Unit.ItemStackView.ToggleExclusiveItemStack(newEvent.ItemType, true);
                        }
                    }
                }
            }
        }
    }
}
