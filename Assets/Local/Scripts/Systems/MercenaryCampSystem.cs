﻿using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Scripts
{
    public class MercenaryCampSystem: ISystem
    {
        public List<Character> Characters;
        public List<Building> Buildings;

        public PoolCollection<Mercenary> MercenaryPools;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (building.ConsumeItemType == Enums.ItemType.NONE && building.ProduceItemType == Enums.ItemType.GOLD)
                {
                    foreach (var character in building.UnloadingCharacters)
                    {
                        if (character.CharacterType == Enums.CharacterType.PLAYER)
                        {
                            if (MercenaryCreate(building, character))
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        public bool MercenaryCreate(Building building, Character character)
        {            
            if (character.NextInQueue != null)
            {
                building.ProductionItemAmountPerCycle += 25;
                building.ProductionLimit = building.ProductionItemAmountPerCycle;

                var recrut = character.NextInQueue;

                var mercenary = MercenaryPools.Get(0);
                Characters.Add(mercenary);
                Characters.Remove(recrut);
                mercenary.transform.position = recrut.transform.position;

                building.ProductionCharacters.Remove(recrut);
                building.UnloadingCharacters.Remove(recrut);
                recrut.LeaveQueue();
                recrut.Release();

                return true;
            }
            return false;
        }

        public void StartRaid() 
        {
            foreach (var building in Buildings)
            {
                if (building.ConsumeItemType == Enums.ItemType.MERCENARY)
                {
                    
                }
            }
        }
    }
}