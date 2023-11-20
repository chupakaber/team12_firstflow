using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BuildingProductionSystem: ISystem
    {
        public EventBus EventBus;

        public List<Building> Buildings;

        public void EventCatch(StartEvent newEvent)
        {
            var buildingsArray = Object.FindObjectsOfType<Building>();
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
                    building.LastProductionTime = Time.time;
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
                    building.LastProductionTime = Time.time;
                }
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (Time.time >= building.LastProductionTime + building.ProductionCooldown)
                {
                    if(building.ProductionArea == null || building.ProductionCharacters.Count > 0)
                    {
                        if (building.ItemCost <= building.Items.GetAmount(building.ConsumeItemType))
                        {
                            if(building.ProductionLimit >= building.Items.GetAmount(building.ProduceItemType) + building.ProductionItemAmountPerCycle)
                            {
                                var addItemEvent = new AddItemEvent() { ItemType = building.ProduceItemType, Count = building.ProductionItemAmountPerCycle, Unit = building };
                                EventBus.CallEvent(addItemEvent);

                                if(building.ProduceSecondItemType != Enums.ItemType.NONE)
                                {
                                    addItemEvent = new AddItemEvent() { ItemType = building.ProduceSecondItemType, Count = building.ProductionSecondItemAmountPerCycle, Unit = building };
                                    EventBus.CallEvent(addItemEvent);
                                }

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
