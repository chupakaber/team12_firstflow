using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BuildingProductionSystem: ISystem
    {
        public EventBus EventBus;
        public Character Character;

        public List<Building> Buildings;

        public void EventCatch(StartEvent newEvent)
        {
            var buildingsArray = GameObject.FindObjectsOfType<Building>();
            foreach (Building building in buildingsArray)
            {
                Buildings.Add(building);
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            Production();

            foreach (var building in Buildings)
            {
                building.IsWork = Character.CollidedWith != null && Character.CollidedWith.Equals(building.ProductionArea);
            }
        }

        private void Production()
        {
            foreach (var building in Buildings)
            {
                if (Time.time >= building.LastProductionTime + 1f && building.IsWork)
                {
                    var addItemEvent = new AddItemEvent() {ItemType = building.ProduceItemType, Count = 1, Character = Character};
                    EventBus.CallEvent(addItemEvent);
                    building.LastProductionTime = Time.time;
                }
            }
        }
    }
}
