using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BuildingConstructionSystem: ISystem
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
                if (building.Level == 0 && building.Levels.Count > 1)
                {
                    foreach (var character in building.ConstructionCharacters)
                    {
                        Collecting(building,character);
                    }
                }
            }
        }

        public void EventCatch(ActivateObjectEvent newEvent)
        {
            if (newEvent.Target is Building)
            {
                var building = (Building) newEvent.Target;
                UpdateUpgradeProgressViewSettings(building);
            }
        }

        public void EventCatch(LevelUpEvent newEvent)
        {
            if (newEvent.Target is Building)
            {
                var building = (Building) newEvent.Target;
                UpdateUpgradeProgressViewSettings(building);
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

                                var i = 0;
                                foreach (var teleportingCharacter in building.ConstructionCharacters)
                                {
                                    var moveDirection = new Vector3(1f, 0f, -1f);
                                    teleportingCharacter.transform.position = building.PickingUpArea == null ? building.ConstructionArea.transform.position + moveDirection * (i + 3) * 1f : building.PickingUpArea.transform.position + moveDirection * i;
                                    i++;
                                }

                                foreach (var item in building.Items)
                                {
                                    EventBus.CallEvent(new RemoveItemEvent() { ItemType = item.Type, Count = item.Amount, Unit = building });
                                }

                                UpdateUpgradeProgressViewSettings(building);

                                EventBus.CallEvent(new PlaySoundEvent() { SoundId = 8, IsMusic = false, AttachedTo = building.transform });
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
            if (building.UpgradeStorage != null && building.Levels.Count > building.Level + 1)
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
            if (building.Level > 0)
            {
                foreach (var progressBar in building.CollectingProgressBars)
                {
                    if (progressBar.ItemType == building.ConsumeItemType)
                    {
                        progressBar.Capacity = building.ItemCost;
                        progressBar.FillValues();
                    }
                }
            }
        }
    }
}
