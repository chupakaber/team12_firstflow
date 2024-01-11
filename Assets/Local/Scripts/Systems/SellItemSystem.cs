using System.Collections.Generic;
using DG.Tweening;
using Scripts.Enums;

namespace Scripts.Systems
{
    public class SellItemSystem: ISystem
    {
        public EventBus EventBus;
        public PoolCollection<ItemView> ItemViewPools;
        public PoolCollection<IconView> IconViewPools;
        public List<Character> Characters;
        public UIView UIView;

        public void EventCatch(RemoveItemEvent newEvent)
        {
            if (newEvent.Unit is Building)
            {
                var building = (Building) newEvent.Unit;
                if (building.ProduceItemType == ItemType.GOLD && newEvent.ItemType != ItemType.GOLD)
                {
                    foreach (var character in Characters)
                    {
                        if (character.CharacterType == CharacterType.CUSTOMER)
                        {
                            var customer = (Customer) character;
                            if (customer.TargetBuilding == building && customer.IsReadyToBuy())
                            {
                                customer.AddItem(newEvent.ItemType, newEvent.Count);
                                for (var i = 0; i < newEvent.Count; i++)
                                {
                                    var itemView = ItemViewPools.Get((int) newEvent.ItemType);
                                    if (itemView != null)
                                    {
                                        customer.ItemStackView.AddItem(itemView);
                                        var endPosition = customer.GetStackTopPosition(newEvent.ItemType);
                                        itemView.gameObject.SetActive(false);
                                        var itemViewForFly = ItemViewPools.Get((int) newEvent.ItemType);
                                        itemViewForFly.transform.position = newEvent.Unit.transform.position;
                                        itemViewForFly.transform.DOJump(endPosition, 1f, 1, 0.5f).OnComplete(() => {
                                            itemViewForFly.Release();
                                            itemView.gameObject.SetActive(true);
                                            customer.ItemStackView.SortItems();
                                        });
                                    }
                                }

                                customer.TargetBuilding = null;
                                customer.LeaveQueue();

                                EventBus.CallEvent(new CustomerReachKeyEvent() { Customer = customer });
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
