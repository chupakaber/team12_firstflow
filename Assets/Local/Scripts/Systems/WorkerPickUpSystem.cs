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

        private float _lastWorkerPickUpTime;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            if (Time.time >= _lastWorkerPickUpTime + 1f)
            {
                _lastWorkerPickUpTime = Time.time;
                foreach (var building in Buildings)
                {
                    if (building.ProduceItemType == ItemType.ASSISTANT || building.ProduceItemType == ItemType.APPRENTICE)
                    {
                        foreach (var player in Characters)
                        {
                            if (player.CharacterType == CharacterType.PLAYER)
                            {
                                foreach (var character in Characters)
                                {
                                    if (
                                        (character.CharacterType == CharacterType.ASSISTANT && building.ProduceItemType == ItemType.ASSISTANT) 
                                        || (character.CharacterType == CharacterType.APPRENTICE && building.ProduceItemType == ItemType.APPRENTICE))
                                    {
                                        var worker = (Worker) character;
                                        if (worker.TargetBuilding == building && worker.PreviousInQueue == null)
                                        {
                                            if (building.Items.GetAmount(building.ProduceItemType) > 0)
                                            {
                                                worker.TargetBuilding = null;
                                                player.AddLastInQueue(worker);
                                                worker.FollowingOffset = 2.2f;

                                                building.UnloadingCharacters.Remove(player);

                                                EventBus.CallEvent(new RemoveItemEvent() { Unit = building, ItemType = building.ProduceItemType, Count = 1 });
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}