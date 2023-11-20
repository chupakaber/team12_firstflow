using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class WorkerPickUpSystem : ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (building.ProduceItemType == ItemType.ASSISTANT || building.ProduceItemType == ItemType.APPRENTICE)
                {
                    foreach (var character in building.PickingUpCharacters)
                    {
                        if (character.CharacterType == CharacterType.PLAYER)
                        {
                            PickUp(building, character);
                        }
                    }
                }
            }
        }

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.PickingUpArea))
                {
                    building.PickingUpCharacters.Add(newEvent.Character);
                }
            }
        }

        public void EventCatch(ExitTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.PickingUpArea))
                {
                    building.PickingUpCharacters.Remove(newEvent.Character);
                }
            }
        }

        private void PickUp(Building building, Character player)
        {
            if (Time.time >= player.LastMoveItemTime + 1f)
            {
                player.LastMoveItemTime = Time.time;

                foreach (var character in Characters)
                {
                    if (character.CharacterType == CharacterType.ASSISTANT)
                    {
                        var worker = (Worker) character;
                        if (worker.TargetBuilding == null && worker.TargetCharacter == null)
                        {
                            worker.TargetCharacter = player;
                        }
                    }
                }
            }
        }
    }
}