using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BuildingUpgradeSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public void EventCatch(StartEvent newEvent) 
        {
            foreach (var building in Buildings)
            {
                if (!building.gameObject.activeSelf)
                {
                    building.UpgradeCharacters.Clear();
                }

                UpdateUpgradeProgressViewSettings(building);
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent) 
        {
            foreach (var building in Buildings)
            {
                foreach (var character in building.UpgradeCharacters)
                {
                    Collecting(building, character);
                }
            }
        }

        private void Collecting(Building building, Character character)
        {
            if (building.UpgradeStorage == null)
            {
                return;
            }

            var characterHorizontalVelocity = character.CharacterRigidbody.velocity;
            characterHorizontalVelocity.y = 0f;
            if (characterHorizontalVelocity.sqrMagnitude > 0.5f)
            {
                character.LastDropItemTime = Time.time;
            }

            if (building.Levels.Count > building.Level + 1)
            {
                var levelConfig = building.Levels[building.Level + 1];
                foreach (var requiredItem in levelConfig.Cost)
                {
                    var storageAmount = building.UpgradeStorage.Items.GetAmount(requiredItem.Type);
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
                            var addItemEvent = new AddItemEvent() { ItemType = requiredItem.Type, Count = itemsMovingAmount, Unit = building.UpgradeStorage, FromPosition = sourcePileTopPosition };
                            EventBus.CallEvent(removeItemEvent);
                            EventBus.CallEvent(addItemEvent);
                            character.LastDropItemTime = Time.time;

                            var completed = true;
                            
                            foreach (var requiredItem2 in levelConfig.Cost)
                            {
                                if (building.UpgradeStorage.Items.GetAmount(requiredItem2.Type) < requiredItem2.Amount)
                                {
                                    completed = false;
                                }
                            }

                            if (completed)
                            {
                                building.Level = building.Level + 1;

                                UpdateUpgradeProgressViewSettings(building);
                                
                                foreach (var item in building.UpgradeStorage.Items)
                                {
                                    EventBus.CallEvent(new RemoveItemEvent() { ItemType = item.Type, Count = item.Amount, Unit = building.UpgradeStorage });
                                }

                                if (building.Levels.Count <= building.Level + 1)
                                {
                                    building.UpgradeArea.gameObject.SetActive(false);
                                    building.UpgradeArea = null;
                                }
                            }
                        }
                    }
                    else if (availableAmount > 0 && requiredAmount > 0)
                    {
                        break;
                    }
                }
            }
        }

        public void UpdateUpgradeProgressViewSettings(Building building)
        {
            if (building.UpgradeStorage == null)
            {
                return;
            }
            
            var currentLevelConfig = building.Levels[building.Level];
            foreach (var boost in currentLevelConfig.Boost)
            {
                ApplyUpgrade(building, boost);
            }


            if (building.Levels.Count > building.Level + 1)
            {
                var nextLevelConfig = building.Levels[building.Level + 1];
                
                foreach (var progressBar in building.UpgradeStorage.CollectingProgressBars)
                {
                    foreach (var requirement in nextLevelConfig.Cost)
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

        public void ApplyUpgrade(Building building, Boost boost)
        {
            switch (boost.Type)
            {
                case Enums.BoostType.PRODUCTION_COOLDOWN_MULTIPLICATOR:
                    building.ProductionCooldown *= boost.Value;
                break;
            }
        }
    }
}
