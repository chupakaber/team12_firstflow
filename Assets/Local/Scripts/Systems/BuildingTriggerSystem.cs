using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BuildingTriggerSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.ConstructionArea) && !building.ConstructionCharacters.Contains(newEvent.Character))
                {
                    building.ConstructionCharacters.Add(newEvent.Character);
                }
                else if (newEvent.Trigger.Equals(building.UnloadingArea) && !building.UnloadingCharacters.Contains(newEvent.Character))
                {
                    building.UnloadingCharacters.Add(newEvent.Character);
                }
                else if (newEvent.Trigger.Equals(building.PickingUpArea) && !building.PickingUpCharacters.Contains(newEvent.Character))
                {
                    building.PickingUpCharacters.Add(newEvent.Character);
                }
                else if (newEvent.Trigger.Equals(building.ProductionArea) && !building.ProductionCharacters.Contains(newEvent.Character))
                {
                    building.ProductionCharacters.Add(newEvent.Character);
                }
                else if (newEvent.Trigger.Equals(building.UpgradeArea) && !building.UpgradeCharacters.Contains(newEvent.Character))
                {
                    building.UpgradeCharacters.Add(newEvent.Character);
                }
            }
        }

        public void EventCatch(ExitTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.ConstructionArea))
                {
                    building.ConstructionCharacters.Remove(newEvent.Character);
                }

                if (newEvent.Trigger.Equals(building.UnloadingArea))
                {
                    building.UnloadingCharacters.Remove(newEvent.Character);
                }

                if (newEvent.Trigger.Equals(building.PickingUpArea))
                {
                    building.PickingUpCharacters.Remove(newEvent.Character);
                }

                if (newEvent.Trigger.Equals(building.ProductionArea))
                {
                    building.ProductionCharacters.Remove(newEvent.Character);
                }

                if (newEvent.Trigger.Equals(building.UpgradeArea))
                {
                    building.UpgradeCharacters.Remove(newEvent.Character);
                }
            }
        }
    }
}
