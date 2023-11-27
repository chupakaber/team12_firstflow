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
                if (newEvent.Trigger.Equals(building.ConstructionArea))
                {
                    building.ConstructionCharacters.Add(newEvent.Character);
                }

                if (newEvent.Trigger.Equals(building.UnloadingArea))
                {
                    building.UnloadingCharacters.Add(newEvent.Character);
                }

                if (newEvent.Trigger.Equals(building.PickingUpArea))
                {
                    building.PickingUpCharacters.Add(newEvent.Character);
                }

                if (newEvent.Trigger.Equals(building.ProductionArea))
                {
                    building.ProductionCharacters.Add(newEvent.Character);
                }

                if (newEvent.Trigger.Equals(building.UpgradeArea))
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
