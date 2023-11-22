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
                        if (building.ConstructionStorage.Items.GetAmount(requirementItem.Type) < requirementItem.Amount)
                        {
                            var removeItemEvent = new RemoveItemEvent() { ItemType = requirementItem.Type, Count = itemsMovingAmount, Unit = character };
                            var addItemEvent = new AddItemEvent() { ItemType = requirementItem.Type, Count = itemsMovingAmount, Unit = building.ConstructionStorage };
                            EventBus.CallEvent(removeItemEvent);
                            EventBus.CallEvent(addItemEvent);

                            var amount = building.ConstructionStorage.Items.GetAmount(requirementItem.Type);
                            if (requirementItemIndex < building.ConstructionProgressBars.Count)
                            {
                                var progressBar = building.ConstructionProgressBars[requirementItemIndex];
                                if (progressBar != null)
                                {
                                    progressBar.localScale = new Vector3(1f, (float) amount / requirementItem.Amount, 1f);
                                    progressBar.localPosition = new Vector3(0f, 0.5f - (float) amount / requirementItem.Amount * 0.5f, 0f);
                                }
                            }

                            if (requirementItemIndex < building.ConstructionPriceLabels.Count)
                            {
                                var label = building.ConstructionPriceLabels[requirementItemIndex];
                                if (label != null)
                                {
                                    label.text = $"{requirementItem.Amount - amount}";
                                }
                            }

                            if (amount >= requirementItem.Amount)
                            {
                                var levelUp = true;
                                foreach (var requirement in building.Levels[1].Cost)
                                {
                                    if (building.ConstructionStorage.Items.GetAmount(requirement.Type) < requirement.Amount)
                                    {
                                        levelUp = false;
                                    }
                                }

                                if (levelUp)
                                {
                                    building.Level = building.Level + 1;
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
