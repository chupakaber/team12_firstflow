using Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Building: Unit
    {
        public Collider ProductionArea;
        public Collider UnloadingArea;
        public Collider PickingUpArea;
        public Collider UpgradeArea;
        public int ProductionLimit;
        public int ResourceLimit;
        public int ItemCost;
        public int ProductionAmountPerCycle;
        public float ProductionCooldown;
        public ItemType ProduceItemType;
        public ItemType ConsumeItemType;

        public List<Character> ProductionCharacters = new List<Character>();
        public List<Character> UnloadingCharacters = new List<Character>();
        public List<Character> PickingUpCharacters = new List<Character>();
        public List<Character> UpgradeCharacters = new List<Character>();
        public float LastProductionTime;
    }
}
