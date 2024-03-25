using System.Collections.Generic;
using Scripts.Enums;

namespace Scripts
{
    public class AddHonorSystem : ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;

        private bool _loading = false;

        public void EventCatch(SaveLoadStateEvent newEvent)
        {
            _loading = newEvent.Loading;
        }

        public void EventCatch(AddItemEvent newEvent)
        {
            if (_loading)
            {
                return;
            }

            if (newEvent.ItemType == ItemType.GOLD && newEvent.Unit is Building)
            {
                var building = (Building) newEvent.Unit;
                if (building.ProduceItemType == ItemType.GOLD && building.Level > 0)
                {
                    for (var i = 0; i < Characters.Count; i++)
                    {
                        var character = Characters[i];
                        if (character.CharacterType == CharacterType.PLAYER)
                        {
                            var honorAmount = building.GetLastCustomerHonor();

                            var addItemEvent = new AddItemEvent() { ItemType = ItemType.HONOR, Count = honorAmount, Unit = character };
                            EventBus.CallEvent(addItemEvent);

                            if (building.HonorIconAnimation != null)
                            {
                                building.HonorIconAnimation.Play();
                            }
                        }
                    }
                }
            }

            if (newEvent.ItemType == ItemType.HONOR && newEvent.Unit is Building)
            {
                var building = (Building)newEvent.Unit;
                if (building.ProduceItemType == ItemType.HONOR)
                {
                    for (var i = 0; i < Characters.Count; i++)
                    {
                        var character = Characters[i];
                        if (character.CharacterType == CharacterType.PLAYER)
                        {
                            var honorAmount = building.ProductionItemAmountPerCycle;

                            var addItemEvent = new AddItemEvent() { ItemType = ItemType.HONOR, Count = honorAmount, Unit = character };
                            EventBus.CallEvent(addItemEvent);

                            var removeItemEvent = new RemoveItemEvent() { ItemType = ItemType.HONOR, Count = honorAmount, Unit = building };
                            EventBus.CallEvent(removeItemEvent);

                            if (building.HonorIconAnimation != null)
                            {
                                building.HonorIconAnimation.Play();
                            }
                        }
                    }
                }
            }
        }
    }
}
