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
                foreach (var character in building.PickingUpCharacters)
                {
                    PickUp(building, character);
                }
            }
        }

        private void PickUp(Building building, Character character)
        {
            var itemsMoveAmount = 1;
            if (Time.time >= character.LastPickUpItemTime + character.PickUpCooldown)
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
                    if (character.ItemLimit >= character.Items.GetAmount() + character.Items.GetItemVolume(building.ProduceItemType) * itemsMoveAmount)
                    {
                        if (building.Items.GetAmount(building.ProduceItemType) >= itemsMoveAmount)
                        {
                            var sourcePileTopPosition = building.GetStackTopPosition();
                            var removeItemEvent = new RemoveItemEvent() { ItemType = building.ProduceItemType, Count = itemsMoveAmount, Unit = building };
                            var addItemEvent = new AddItemEvent() { ItemType = building.ProduceItemType, Count = itemsMoveAmount, Unit = character, FromPosition = sourcePileTopPosition };
                            EventBus.CallEvent(removeItemEvent);
                            EventBus.CallEvent(addItemEvent);
                            character.LastPickUpItemTime = Time.time;
                        }
                    }
                }
            }
        }
    }
}