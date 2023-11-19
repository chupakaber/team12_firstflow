using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts
{
    public class BuildingCollectingSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public void EventCatch(FixedUpdateEvent newEvent) 
        {
            foreach (var building in Buildings)
            {
                foreach (var character in building.UnloadingCharacters)
                {
                    Collecting(building,character);
                }
            }
        }

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.UnloadingArea))
                {
                    building.UnloadingCharacters.Add(newEvent.Character);
                }
            }
        }

        public void EventCatch(ExitTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.UnloadingArea))
                {
                    building.UnloadingCharacters.Remove(newEvent.Character);
                }
            }
        }

        private void Collecting(Building building, Character character)
        {
            var itemsMovingAmount = 1;
            
            if (character.CharacterRigidbody.velocity.sqrMagnitude > 0.1f) {
                character.LastMoveItemTime = Time.time;
            }

            if (Time.time >= character.LastMoveItemTime + character.PickUpCooldown)
            {
                if (character.Items.GetAmount(building.ConsumeItemType) >= itemsMovingAmount)
                {
                    if (building.Items.GetAmount(building.ConsumeItemType) < building.ResourceLimit)
                    {
                        var removeItemEvent = new RemoveItemEvent() { ItemType = building.ConsumeItemType, Count = itemsMovingAmount, Unit = character };
                        var addItemEvent = new AddItemEvent() { ItemType = building.ConsumeItemType, Count = itemsMovingAmount, Unit = building };
                        EventBus.CallEvent(removeItemEvent);
                        EventBus.CallEvent(addItemEvent);
                        character.LastMoveItemTime = Time.time;
                    }
                }
            }            
        }
    }
}
