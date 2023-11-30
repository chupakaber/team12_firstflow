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

        private void PickUp(Building building, Character player)
        {
            if (Time.time >= player.LastPickUpItemTime + 0.1f)
            {
                player.LastPickUpItemTime = Time.time;

                foreach (var character in Characters)
                {
                    if (
                        (character.CharacterType == CharacterType.ASSISTANT && building.ProduceItemType == ItemType.ASSISTANT) 
                        || (character.CharacterType == CharacterType.APPRENTICE && building.ProduceItemType == ItemType.APPRENTICE))
                    {
                        var worker = (Worker) character;
                        if (worker.TargetBuilding == building && worker.TargetCharacter == null)
                        {
                            if (building.Items.GetAmount(building.ProduceItemType) > 0)
                            {
                                EventBus.CallEvent(new RemoveItemEvent() { Unit = building, ItemType = building.ProduceItemType, Count = 1 });

                                worker.TargetBuilding = null;
                                worker.TargetCharacter = player;
                                worker.FollowingOffset = 2f;
                            }
                        }
                    }
                }
            }
        }
    }
}