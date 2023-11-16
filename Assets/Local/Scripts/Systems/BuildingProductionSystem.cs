using System.Collections.Generic;
using UnityEngine;

namespace Assets.Local.Scripts
{
    public class BuildingProductionSystem
    {
        public List<Building> Buildings = new List<Building>();

        
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
                    Debug.Log("создал");
                    building.LastProductionTime = Time.time;
                }
            }
        }
    }
}
