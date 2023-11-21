using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class PickUpSystem : ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                foreach (var character in building.PickingUpCharacters)
                {
                    PickUp(building, character);
                }
            }
        }

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.PickingUpArea))
                {
                    building.PickingUpCharacters.Add(newEvent.Character);
                }
            }
        }

        public void EventCatch(ExitTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.PickingUpArea))
                {
                    building.PickingUpCharacters.Remove(newEvent.Character);
                }
            }
        }

        private void PickUp(Building building, Character character)
        {
            var itemsMoveAmount = 1;
            if (Time.time >= character.LastMoveItemTime + character.PickUpCooldown)
            {
                if (character.PickUpItemConstraint == Enums.ItemType.NONE || character.PickUpItemConstraint == building.ProduceItemType)
                {
                    if (character.ItemLimit >= character.Items.GetAmount() + character.Items.GetItemVolume(building.ProduceItemType) * itemsMoveAmount)
                    {
                        if (building.Items.GetAmount(building.ProduceItemType) >= itemsMoveAmount)
                        {
                            var removeItemEvent = new RemoveItemEvent() { ItemType = building.ProduceItemType, Count = itemsMoveAmount, Unit = building };
                            var addItemEvent = new AddItemEvent() { ItemType = building.ProduceItemType, Count = itemsMoveAmount, Unit = character };
                            EventBus.CallEvent(removeItemEvent);
                            EventBus.CallEvent(addItemEvent);
                            character.LastMoveItemTime = Time.time;
                        }

                        if (building.Items.GetAmount(building.ProduceSecondItemType) >= itemsMoveAmount)
                        {
                            var removeItemEvent = new RemoveItemEvent() { ItemType = building.ProduceSecondItemType, Count = itemsMoveAmount, Unit = building };
                            var addItemEvent = new AddItemEvent() { ItemType = building.ProduceSecondItemType, Count = itemsMoveAmount, Unit = character };
                            EventBus.CallEvent(removeItemEvent);
                            EventBus.CallEvent(addItemEvent);
                            character.LastMoveItemTime = Time.time;
                        }
                    }
                }
            }
        }
    }
}