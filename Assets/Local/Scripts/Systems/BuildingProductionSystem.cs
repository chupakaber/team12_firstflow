using Scripts.Enums;
using Scripts.Systems;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BuildingProductionSystem
    {
        public List<Building> Buildings = new List<Building>();
        public CraftSystem CraftSystem;
        public Character character;

        
        public void Init()
        {
            var buildingsArray = GameObject.FindObjectsOfType<Building>();
            Buildings = new List<Building>(buildingsArray);
        }

        public void Production()
        {
            foreach (var building in Buildings)
            {
                if (Time.time >= building.LastProductionTime + 1f && building.IsWork)
                {
                    CraftSystem.AddItem(building.ItemType, 1, character);
                    Debug.Log("создал");
                    building.LastProductionTime = Time.time;
                }
            }
        }
    }
}
