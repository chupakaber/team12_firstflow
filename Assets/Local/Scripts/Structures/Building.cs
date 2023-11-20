using Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Building: Unit
    {
        [Header("Building Config")]
        public Collider ProductionArea;
        public Collider UnloadingArea;
        public Collider PickingUpArea;
        public Collider UpgradeArea;
        public int ProductionLimit;
        public int ResourceLimit;
        public int ItemCost;
        public int ProductionItemAmountPerCycle;
        public int ProductionSecondItemAmountPerCycle;
        public float ProductionCooldown;
        public ItemType ProduceItemType;
        public ItemType ProduceSecondItemType;
        public ItemType ConsumeItemType;

        [Header("Building Runtime")]
        public List<Character> ProductionCharacters = new List<Character>();
        public List<Character> UnloadingCharacters = new List<Character>();
        public List<Character> PickingUpCharacters = new List<Character>();
        public List<Character> UpgradeCharacters = new List<Character>();
        public float LastProductionTime;
    }
}
