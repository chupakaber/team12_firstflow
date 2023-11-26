using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BuildingConstructionSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public void EventCatch(FixedUpdateEvent newEvent) 
        {
            foreach (var building in Buildings)
            {
                if (building.Level == 0 && building.Levels.Count > 1)
                {
                    foreach (var character in building.ConstructionCharacters)
                    {
                        Collecting(building,character);
                    }
                }
            }
        }

        private void Collecting(Building building, Character character)
        {
            var itemsMovingAmount = 1;

            var characterHorizontalVelocity = character.CharacterRigidbody.velocity;
            characterHorizontalVelocity.y = 0f;
            if (characterHorizontalVelocity.sqrMagnitude > 0.1f)
            {
                character.LastMoveItemTime = Time.time;
            }

            if (Time.time >= character.LastMoveItemTime + 0.06f)
            {
                var requirementItemIndex = 0;
                foreach (var requirementItem in building.Levels[1].Cost)
                {
                    if (character.Items.GetAmount(requirementItem.Type) >= itemsMovingAmount)
                    {
                        if (building.Items.GetAmount(requirementItem.Type) < requirementItem.Amount)
                        {
                            var removeItemEvent = new RemoveItemEvent() { ItemType = requirementItem.Type, Count = itemsMovingAmount, Unit = character };
                            var addItemEvent = new AddItemEvent() { ItemType = requirementItem.Type, Count = itemsMovingAmount, Unit = building };
                            EventBus.CallEvent(removeItemEvent);
                            EventBus.CallEvent(addItemEvent);

                            var amount = building.Items.GetAmount(requirementItem.Type);

                            if (amount >= requirementItem.Amount)
                            {
                                var levelUp = true;
                                foreach (var requirement in building.Levels[1].Cost)
                                {
                                    if (building.Items.GetAmount(requirement.Type) < requirement.Amount)
                                    {
                                        levelUp = false;
                                    }
                                }

                                if (levelUp)
                                {
                                    building.Level = building.Level + 1;
                                    foreach (var item in building.Items)
                                    {
                                        EventBus.CallEvent(new RemoveItemEvent() { ItemType = item.Type, Count = item.Amount, Unit = building });
                                    }
                                }
                            }

                            character.LastMoveItemTime = Time.time;
                            break;
                        }
                    }
                    requirementItemIndex++;
                }
            }            
        }
    }
}
