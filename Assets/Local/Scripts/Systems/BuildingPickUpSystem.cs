using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class BuildingPickUpSystem : ISystem
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
                    building.PickingUpCharacters.Clear();
                }

                foreach (var character in building.PickingUpCharacters)
                {
                    PickUp(building, character);
                }
            }
        }

        private void PickUp(Building building, Character character)
        {
            var availableAmount = building.Items.GetAmount(building.ProduceItemType);

            if (availableAmount < 1)
            {
                return;
            }

            var storageAmount = character.Items.GetAmount();
            var pickUpCooldown = character.GetPickUpCooldown(building.ProduceItemType, out var itemsMovingAmount, availableAmount);

            if (Time.time >= character.LastPickUpItemTime + pickUpCooldown)
            {
                if (building.ProduceItemType == ItemType.ASSISTANT || building.ProduceItemType == ItemType.APPRENTICE)
                {
                    return;
                }

                if (building.ProduceItemType == ItemType.GOLD && character.CharacterType != CharacterType.PLAYER)
                {
                    return;
                }

                if (character.PickUpItemConstraint == ItemType.NONE || character.PickUpItemConstraint == building.ProduceItemType)
                {
                    itemsMovingAmount = Mathf.Min(itemsMovingAmount, availableAmount);
                    if (itemsMovingAmount > 0 && character.ItemLimit >= storageAmount + character.Items.GetItemVolume(building.ProduceItemType) * itemsMovingAmount)
                    {
                        var sourcePileTopPosition = building.GetStackTopPosition(building.ProduceItemType);
                        var removeItemEvent = new RemoveItemEvent() { ItemType = building.ProduceItemType, Count = itemsMovingAmount, Unit = building };
                        var addItemEvent = new AddItemEvent() { ItemType = building.ProduceItemType, Count = itemsMovingAmount, Unit = character, FromPosition = sourcePileTopPosition };
                        EventBus.CallEvent(removeItemEvent);
                        EventBus.CallEvent(addItemEvent);
                        character.LastPickUpItemTime = Time.time;
                    }
                }
            }
        }
    }
}