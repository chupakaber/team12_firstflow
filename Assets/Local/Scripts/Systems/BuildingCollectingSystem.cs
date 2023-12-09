using System.Collections.Generic;
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
                if (!building.gameObject.activeSelf)
                {
                    building.UnloadingCharacters.Clear();
                }
                
                foreach (var character in building.UnloadingCharacters)
                {
                    Collecting(building, character);
                }
            }
        }

        private void Collecting(Building building, Character character)
        {
            var characterHorizontalVelocity = character.CharacterRigidbody.velocity;
            characterHorizontalVelocity.y = 0f;
            if (characterHorizontalVelocity.sqrMagnitude > 0.5f)
            {
                character.LastDropItemTime = Time.time;
            }

            var dropCooldown = character.GetDropCooldown(building.ConsumeItemType, out var itemsMovingAmount, building.ResourceLimit);

            if (Time.time >= character.LastDropItemTime + dropCooldown)
            {
                var storageAmount = building.Items.GetAmount(building.ConsumeItemType);
                var requiredAmount = building.ResourceLimit - storageAmount;
                var availableAmount = character.Items.GetAmount(building.ConsumeItemType);
                itemsMovingAmount = Mathf.Min(itemsMovingAmount, Mathf.Min(requiredAmount, availableAmount));
                if (itemsMovingAmount > 0)
                {
                    var sourcePileTopPosition = character.GetStackTopPosition(building.ConsumeItemType);
                    var removeItemEvent = new RemoveItemEvent() { ItemType = building.ConsumeItemType, Count = itemsMovingAmount, Unit = character };
                    var addItemEvent = new AddItemEvent() { ItemType = building.ConsumeItemType, Count = itemsMovingAmount, Unit = building, FromPosition = sourcePileTopPosition };
                    EventBus.CallEvent(removeItemEvent);
                    EventBus.CallEvent(addItemEvent);
                    character.LastDropItemTime = Time.time;
                }
            }            
        }
    }
}
