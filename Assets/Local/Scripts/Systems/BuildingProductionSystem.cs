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
                    foreach (var character in building.ProductionCharacters) {
                        if (character.CharacterRigidbody.velocity.sqrMagnitude > 0.1f) {
                            building.LastProductionTime = Time.time;
                            break;
                        }
                    }

                    if (Time.time < building.LastProductionTime + building.ProductionCooldown) {
                        break;
                    }

                    if(building.ProductionArea == null || building.ProductionCharacters.Count > 0)
                    {
                        if (building.ItemCost <= building.Items.GetAmount(building.ConsumeItemType))
                        {
                            if(building.ProductionLimit >= building.Items.GetAmount(building.ProduceItemType) + building.ProductionAmountPerCycle)
                            {
                                var addItemEvent = new AddItemEvent() { ItemType = building.ProduceItemType, Count = building.ProductionAmountPerCycle, Unit = building };
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
