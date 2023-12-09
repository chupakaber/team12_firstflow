using System.Collections.Generic;
using Scripts.Enums;

namespace Scripts
{
    public class AddHonorSystem : ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;

        public void EventCatch(AddItemEvent newEvent)
        {
            if (newEvent.ItemType == ItemType.GOLD && newEvent.Unit is Building)
            {
                var building = (Building) newEvent.Unit;
                if (building.ProduceItemType == ItemType.GOLD && building.Level > 0)
                {
                    foreach (var character in Characters)
                    {
                        if (character.CharacterType == CharacterType.PLAYER)
                        {
                            var honorAmount = building.GetLastCustomerHonor();

                            var addItemEvent = new AddItemEvent() { ItemType = ItemType.HONOR, Count = honorAmount, Unit = character };
                            EventBus.CallEvent(addItemEvent);
                        }
                    }
                }
            }

            if (newEvent.ItemType == ItemType.HONOR && newEvent.Unit is Building)
            {
                var building = (Building)newEvent.Unit;
                if (building.ProduceItemType == ItemType.HONOR)
                {
                    foreach (var character in Characters)
                    {
                        if (character.CharacterType == CharacterType.PLAYER)
                        {
                            var honorAmount = building.ProductionItemAmountPerCycle;

                            var addItemEvent = new AddItemEvent() { ItemType = ItemType.HONOR, Count = honorAmount, Unit = character };
                            EventBus.CallEvent(addItemEvent);

                            var removeItemEvent = new RemoveItemEvent() { ItemType = ItemType.HONOR, Count = honorAmount, Unit = building };
                            EventBus.CallEvent(removeItemEvent);
                        }
                    }
                }
            }
        }
    }
}
