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
                    var stackHeight = newEvent.Unit.ItemStackView.Count * newEvent.Unit.ItemStackView.Offset;
                    var endPosition = newEvent.Unit.ItemStackView.transform.position + Vector3.up * stackHeight;
                    if (newEvent.FromPosition.sqrMagnitude > float.Epsilon)
                    {
                        var itemViewForFly = ItemViewPools.Get((int) newEvent.ItemType);
                        itemViewForFly.transform.position = newEvent.FromPosition;
                        itemViewForFly.transform.DOJump(endPosition, 1f, 1, 0.5f).OnComplete(() => {
                            itemViewForFly.Release();
                            newEvent.Unit.ItemStackView.AddItem(itemView);
                        });
                    }
                    else
                    {
                        newEvent.Unit.ItemStackView.AddItem(itemView);
                    }
                }
                else
                {
                    if (newEvent.ItemType == ItemType.GOLD && newEvent.Unit is Character && ((Character) newEvent.Unit).CharacterType == CharacterType.PLAYER)
                    {
                        var icon = IconViewPools.Get(0);
                        UIView.FlyCoin(icon, true);
                    }
                }
            }
        }
    }
}
