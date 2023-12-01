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
            var characterHorizontalVelocity = character.CharacterRigidbody.velocity;
            characterHorizontalVelocity.y = 0f;
            if (characterHorizontalVelocity.sqrMagnitude > 0.1f)
            {
                character.LastDropItemTime = Time.time;
            }

            var requiredItemIndex = 0;
            foreach (var requiredItem in building.Levels[1].Cost)
            {
                var storageAmount = building.Items.GetAmount(requiredItem.Type);
                var requiredAmount = requiredItem.Amount - storageAmount;
                var availableAmount = character.Items.GetAmount(requiredItem.Type);
                var dropCooldown = character.GetDropCooldown(requiredItem.Type, out var itemsMovingAmount, requiredAmount);

                if (Time.time >= character.LastDropItemTime + dropCooldown)
                {
                    itemsMovingAmount = Mathf.Min(itemsMovingAmount, Mathf.Min(requiredAmount, availableAmount));

                    if (itemsMovingAmount > 0)
                    {
                        var sourcePileTopPosition = character.GetStackTopPosition(requiredItem.Type);
                        var removeItemEvent = new RemoveItemEvent() { ItemType = requiredItem.Type, Count = itemsMovingAmount, Unit = character };
                        var addItemEvent = new AddItemEvent() { ItemType = requiredItem.Type, Count = itemsMovingAmount, Unit = building, FromPosition = sourcePileTopPosition };
                        EventBus.CallEvent(removeItemEvent);
                        EventBus.CallEvent(addItemEvent);

                        var amount = building.Items.GetAmount(requiredItem.Type);

                        if (amount >= requiredItem.Amount)
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

                                EventBus.CallEvent(new ConstructionCompleteEvent() { Building = building });

                                foreach (var teleportingCharacter in building.ConstructionCharacters)
                                {
                                    teleportingCharacter.transform.position = building.PickingUpArea.transform.position;
                                }

                                foreach (var item in building.Items)
                                {
                                    EventBus.CallEvent(new RemoveItemEvent() { ItemType = item.Type, Count = item.Amount, Unit = building });
                                }

                                UpdateUpgradeProgressViewSettings(building);
                            }
                        }

                        character.LastDropItemTime = Time.time;
                        break;
                    }
                    requiredItemIndex++;
                }
                else if (availableAmount > 0 && requiredAmount > 0)
                {
                    break;
                }
            }            
        }

        public void UpdateUpgradeProgressViewSettings(Building building)
        {
            if (building.Levels.Count > building.Level + 1)
            {
                var levelConfig = building.Levels[building.Level + 1];
                
                foreach (var progressBar in building.UpgradeStorage.CollectingProgressBars)
                {
                    foreach (var requirement in levelConfig.Cost)
                    {
                        if (progressBar.ItemType == requirement.Type)
                        {
                            progressBar.Capacity = requirement.Amount;
                            progressBar.FillValues();
                        }
                    }
                }
            }
        }
    }
}
