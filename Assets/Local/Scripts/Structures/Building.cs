using Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scripts
{
    public class Building: Unit
    {
        [Header("Building Config")]
        public Collider ProductionArea;
        public Collider UnloadingArea;
        public Collider PickingUpArea;
        public Collider UpgradeArea;
        public Transform ProgressBarPivot;
        public int ProductionLimit;
        public int ResourceLimit;
        public int ItemCost;
        public int ProductionItemAmountPerCycle;
        public float ProductionCooldown;
        public float ProductionEndTime;
        public ItemType ProduceItemType;
        public ItemType ConsumeItemType;

        [Header("Building Runtime")]
        public List<Character> ProductionCharacters = new List<Character>();
        public List<Character> UnloadingCharacters = new List<Character>();
        public List<Character> PickingUpCharacters = new List<Character>();
        public List<Character> UpgradeCharacters = new List<Character>();
        public List<Character> AssignedProductionCharacters = new List<Character>();
        public List<Character> AssignedUnloadingCharacters = new List<Character>();
        public List<Character> AssignedPickingUpCharacters = new List<Character>();
        public float LastProductionTime;

        public int GetLastCustomerHonor()
        {
            return 1;
        }

        public float ProductionProgress()
        {
            if (ProductionEndTime > LastProductionTime)
            {
                return (ProductionEndTime - LastProductionTime) / ProductionCooldown;
            }
            else
            {
                var currentProductionTime = Time.time - LastProductionTime;
                return currentProductionTime / ProductionCooldown;
            }
        }
    }
}
