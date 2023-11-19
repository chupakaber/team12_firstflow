using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts
{
    public class BuildingProductionSystem: ISystem
    {
        public EventBus EventBus;

        public List<Building> Buildings;

        public void EventCatch(StartEvent newEvent)
        {
            var buildingsArray = GameObject.FindObjectsOfType<Building>();
            foreach (Building building in buildingsArray)
            {
                Buildings.Add(building);
            }
        }

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.ProductionArea))
                {
                    building.ProductionCharacters.Add(newEvent.Character);
                }
            }
        }

        public void EventCatch(ExitTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.ProductionArea))
                {
                    building.ProductionCharacters.Remove(newEvent.Character);
                }
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (Time.time >= building.LastProductionTime + 1f)
                {
                    if(building.ProductionArea == null || building.ProductionCharacters.Count > 0)
                    {
                        if (building.ItemCost <= building.Items.GetAmount(building.ConsumeItemType))
                        {
                            if(building.ProductionLimit > building.Items.GetAmount(building.ProduceItemType))
                            {
                                var addItemEvent = new AddItemEvent() { ItemType = building.ProduceItemType, Count = 1, Unit = building };
                                EventBus.CallEvent(addItemEvent);

                                if (building.ItemCost > 0)
                                {
                                    var removeItemEvent = new RemoveItemEvent() { ItemType = building.ConsumeItemType, Count = building.ItemCost, Unit = building };
                                    EventBus.CallEvent(removeItemEvent);
                                }

                                building.LastProductionTime = Time.time;
                            }
                        }
                    }
                }  
            }
        }
    }
}
