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
            var itemsMovingAmount = 1;

            var characterHorizontalVelocity = character.CharacterRigidbody.velocity;
            characterHorizontalVelocity.y = 0f;
            if (characterHorizontalVelocity.sqrMagnitude > 0.5f)
            {
                character.LastMoveItemTime = Time.time;
            }

            if (Time.time >= character.LastMoveItemTime + character.PickUpCooldown)
            {
                if (building.Levels.Count > building.Level + 1)
                {
                    var levelConfig = building.Levels[building.Level + 1];
                    foreach (var requiredItem in levelConfig.Cost)
                    {
                        if (character.Items.GetAmount(requiredItem.Type) >= itemsMovingAmount)
                        {
                            if (building.UpgradeStorage.Items.GetAmount(requiredItem.Type) < requiredItem.Amount)
                            {
                                var sourcePileTopPosition = character.GetStackTopPosition();
                                var removeItemEvent = new RemoveItemEvent() { ItemType = requiredItem.Type, Count = itemsMovingAmount, Unit = character };
                                var addItemEvent = new AddItemEvent() { ItemType = requiredItem.Type, Count = itemsMovingAmount, Unit = building.UpgradeStorage, FromPosition = sourcePileTopPosition };
                                EventBus.CallEvent(removeItemEvent);
                                EventBus.CallEvent(addItemEvent);
                                character.LastMoveItemTime = Time.time;

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
                    }
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

                foreach (var boost in levelConfig.Boost)
                {
                    ApplyUpgrade(building, boost);
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
