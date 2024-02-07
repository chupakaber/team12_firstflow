using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class BuildingProductionSystem: ISystem
    {
        public EventBus EventBus;
        public List<Building> Buildings;
        public List<Character> Characters;

        private const float PRODUCTION_CHECK_COOLDOWN = 0.04f;

        public void EventCatch(StartEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                UpdateProductionStateIcon(building);
            }
        }

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (!building.gameObject.activeSelf)
                {
                    building.ProductionCharacters.Clear();
                }

                if (newEvent.Trigger.Equals(building.ProductionArea))
                {
                    if (building.ProductionCharacters.Count == 1)
                    {
                        building.IsWork = true;

                        if (building.ProductionUseBagOfTries)
                        {
                            newEvent.Character.ShowBagOfTries();
                        }

                        if (building.ProductionEndTime > building.LastProductionTime)
                        {
                            building.LastProductionTime = Time.time - (building.ProductionEndTime - building.LastProductionTime);
                            building.ProductionEndTime = building.LastProductionTime - 1f;
                        }
                        else
                        {
                            building.ProductionEndTime = Time.time;
                            building.LastProductionTime = Time.time - (building.ProductionEndTime - building.LastProductionTime);
                        }
                    }

                    TryRestoreTries(building);
                }
            }
        }

        public void EventCatch(ExitTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.ProductionArea))
                {
                    newEvent.Character.HideBagOfTries();
                    if (building.ProductionCharacters.Count == 0)
                    {
                        building.ProductionEndTime = Time.time;
                        building.IsWork = false;
                    }
                    else
                    {
                        building.ProductionCharacters[0].ShowBagOfTries();
                    }
                }
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                foreach (var character in building.ProductionCharacters)
                {
                    // var rotationToBuilding = building.ProductionArea.transform.rotation;
                    var rotationToBuilding = Quaternion.LookRotation(building.transform.position - building.ProductionArea.transform.position, Vector3.up);
                    if (character.CharacterRigidbody.velocity.sqrMagnitude < 0.1f)
                    {
                        character.transform.rotation = Quaternion.Lerp(character.transform.rotation, rotationToBuilding, Time.fixedDeltaTime * 5f);
                        if (character.CharacterType == CharacterType.PLAYER && character.Equals(building.ProductionCharacters[0]))
                        {
                            var deltaPosition = building.ProductionArea.transform.position - character.transform.position;
                            deltaPosition.y = 0f;
                            character.transform.position += deltaPosition * Time.fixedDeltaTime * 5f;
                        }
                    }
                }

                if (Time.time < building.LastProductionCheckTime + PRODUCTION_CHECK_COOLDOWN)
                {
                    continue;
                }

                if (building.Level < 1)
                {
                    continue;
                }

                building.IsWork = false;

                UpdateProductionStateIcon(building);

                if (building.ProduceMethod == Building.ProductionMethod.RESOURCE_TO_TIME && Time.time > building.ProductionEndActivityTime)
                {
                    continue;
                }

                if(building.ProductionArea != null && building.ProductionCharacters.Count < 1)
                {
                    OnStopProduction(building);
                    continue;
                }

                if (building.ItemCost > building.Items.GetAmount(building.ConsumeItemType))
                {
                    OnStopProduction(building);
                    continue;
                }

                if(building.ProductionLimit < building.Items.GetAmount(building.ProduceItemType) + building.ProductionItemAmountPerCycle)
                {
                    OnStopProduction(building);
                    continue;
                }

                building.IsWork = true;
                
                if (building.ProductionCharacters.Count > 0)
                {
                    building.ProductionCharacters[0].ShowBagOfTries();
                }

                if (Time.time < building.LastProductionTime + building.ProductionCooldown)
                {
                    continue;
                }

                var success = true;

                if (building.ProductionUseBagOfTries && building.ProductionCharacters.Count > 0)
                {
                    var character = building.ProductionCharacters[0];
                    if (character.BagOfTries.TryGetNext(out var lastIndex, out var changedValue, out var nextIndex, out var nextValue))
                    {
                        EventBus.CallEvent(new RollBagOfTriesEvent() { Character = character, LastIndex = lastIndex, ChangedValue = changedValue, NextIndex = nextIndex, NextValue = nextValue});
                        success = nextValue;
                    }

                    if (!success)
                    {
                        EventBus.CallEvent(new PlaySoundEvent() { SoundId = 6, IsMusic = false, AttachedTo = character.transform });
                    }

                    TryRestoreTries(building);
                }

                if (building.ItemCost > 0)
                {
                    var removeItemEvent = new RemoveItemEvent() { ItemType = building.ConsumeItemType, Count = building.ItemCost, Unit = building };
                    EventBus.CallEvent(removeItemEvent);
                }

                if (success)
                {
                    var sourcePileTopPosition = building.transform.position;
                    var addItemEvent = new AddItemEvent() { ItemType = building.ProduceItemType, Count = building.ProductionItemAmountPerCycle, Unit = building, FromPosition = sourcePileTopPosition };
                    EventBus.CallEvent(addItemEvent);
                }

                if (building.ProduceItemType == ItemType.SPEAR || building.ProduceItemType == ItemType.SWORD || building.ProduceItemType == ItemType.GUN)
                {
                    if (CheckSoundTime(building, 5f))
                    {
                        EventBus.CallEvent(new PlaySoundEvent() { SoundId = 2, IsMusic = false, AttachedTo = building.transform });
                    }
                }
                else if (building.ProduceItemType == ItemType.IRON || building.ProduceItemType == ItemType.POWDER)
                {
                    if (CheckSoundTime(building, 8f))
                    {
                        EventBus.CallEvent(new PlaySoundEvent() { SoundId = 12, IsMusic = false, AttachedTo = building.transform });
                    }
                }
                else if (building.ProduceItemType == ItemType.WOOD)
                {
                    if (CheckSoundTime(building, 19f))
                    {
                        EventBus.CallEvent(new PlaySoundEvent() { SoundId = 14, IsMusic = false, AttachedTo = building.transform });
                    }
                }
                else if (building.ProduceItemType == ItemType.ROCK)
                {
                    if (CheckSoundTime(building, 7f))
                    {
                        EventBus.CallEvent(new PlaySoundEvent() { SoundId = 17, IsMusic = false, AttachedTo = building.transform });
                    }
                }
                else if (building.ProduceItemType == ItemType.GOLD && building.ProductionArea == null)
                {
                    foreach (var character in Characters)
                    {
                        if (character.CharacterType == CharacterType.PLAYER)
                        {
                            EventBus.CallEvent(new PlaySoundEvent() { SoundId = 15, IsMusic = false, AttachedTo = character.transform, FadeMusic = true });
                        }
                    }
                }

                building.LastProductionTime = Time.time;
            }
        }

        private void OnStopProduction(Building building)
        {
            if (building.ProductionCharacters.Count > 0)
            {
                building.ProductionCharacters[0].HideBagOfTries();
            }
        }

        private void UpdateProductionStateIcon(Building building)
        {
            if (building.ProductionAreaIndicator != null)
            {
                building.IsWorkAreaIndicatorEnabled = building.AssignedProductionCharacters.Count < 1;
            }

            var inProgress = building.Level > 0 && building.ProductionCharacters.Count > 0 && building.ProductionLimit > building.Items.GetAmount(building.ProduceItemType);
            var noResource = building.Level > 0 && building.Items.GetAmount(building.ConsumeItemType) < building.ItemCost;

            if (building.StopProductionIcon != null)
            {
                building.StopProductionIcon.SetActive(!inProgress && !noResource && building.Level > 0);
            }

            if (building.NoResourceIcon != null)
            {
                building.NoResourceIcon.SetActive(noResource);
            }
        }

        private bool CheckSoundTime(Building building, float cooldown = 1f)
        {
            if (Time.time > building.LastProductionSoundTime + cooldown)
            {
                building.LastProductionSoundTime = Time.time;
                return true;
            }

            return false;
        }

        private void TryRestoreTries(Building building)
        {
            var playerInArea = false;
            
            foreach (var character in building.ProductionCharacters)
            {
                if (character.CharacterType == Enums.CharacterType.PLAYER)
                {
                    playerInArea = true;
                }
            }

            if (playerInArea)
            {
                foreach (var character in building.ProductionCharacters)
                {
                    character.RestoreActionTries();
                }
            }
        }
    }
}
