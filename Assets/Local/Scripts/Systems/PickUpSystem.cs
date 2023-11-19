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
            if (Time.time >= character.LastMoveItemTime + 1f)
            {
                if (character.ItemLimit >= character.Items.GetAmount() + 1)
                {
                    if (building.Items.GetAmount(building.ProduceItemType) >= 1)
                    {
                        var removeItemEvent = new RemoveItemEvent() { ItemType = building.ProduceItemType, Count = 1, Unit = building };
                        var addItemEvent = new AddItemEvent() { Count = 1, ItemType = building.ProduceItemType, Unit = character };
                        EventBus.CallEvent(removeItemEvent);
                        EventBus.CallEvent(addItemEvent);
                        character.LastMoveItemTime = Time.time;
                    }
                }
            }
        }
    }
}