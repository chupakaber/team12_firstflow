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
                        newEvent.Character.ShowBagOfTries();
                        if (building.ProductionEndTime > building.LastProductionTime)
                        {
                            building.LastProductionTime = Time.time - (building.ProductionEndTime - building.LastProductionTime);
                            building.ProductionEndTime = building.LastProductionTime - 1f;
                        }
                        else
                        {
                            building.LastProductionTime = Time.time;
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
                if (building.Level < 1)
                {
                    continue;
                }

                if (Time.time < building.LastProductionTime + building.ProductionCooldown)
                {
                    continue;
                }

                if(building.ProductionArea != null && building.ProductionCharacters.Count < 1)
                {
                    continue;
                }

                if (building.ItemCost > building.Items.GetAmount(building.ConsumeItemType))
                {
                    continue;
                }

                if(building.ProductionLimit >= building.Items.GetAmount(building.ProduceItemType) + building.ProductionItemAmountPerCycle)
                {
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

                    if (success)
                    {
                        var sourcePileTopPosition = building.transform.position;
                        var addItemEvent = new AddItemEvent() { ItemType = building.ProduceItemType, Count = building.ProductionItemAmountPerCycle, Unit = building, FromPosition = sourcePileTopPosition };
                        EventBus.CallEvent(addItemEvent);
                    }

                    if (building.ItemCost > 0)
                    {
                        var removeItemEvent = new RemoveItemEvent() { ItemType = building.ConsumeItemType, Count = building.ItemCost, Unit = building };
                        EventBus.CallEvent(removeItemEvent);
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
