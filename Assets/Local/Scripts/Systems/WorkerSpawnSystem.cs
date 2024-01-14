using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class WorkerSpawnSystem: ISystem
    {
        public List<Character> Characters;
        public List<Building> Buildings;

        public PoolCollection<Assistant> AssistantPools;
        public PoolCollection<Apprentice> ApprenticePools;

        private float _lastCheckTime = -3f;
        private Vector3 _workerSpawnPoint;

        public void EventCatch(StartEvent newEvent)
        {
            _workerSpawnPoint = GameObject.Find("WorkerSpawnPoint").transform.position;
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            if (Time.time > _lastCheckTime + 3f) {
                _lastCheckTime = Time.time;

                var freeAssistantsCount = 0;
                var freeApprenticesCount = 0;
                foreach (var character in Characters)
                {
                    if (character.CharacterType == CharacterType.ASSISTANT)
                    {
                        var worker = (Worker) character;
                        if (worker.TargetBuilding != null && worker.TargetBuilding.ProduceItemType == ItemType.ASSISTANT)
                        {
                            freeAssistantsCount++;
                        }
                    }

                    if (character.CharacterType == CharacterType.APPRENTICE)
                    {
                        var worker = (Worker) character;
                        if (worker.TargetBuilding != null && worker.TargetBuilding.ProduceItemType == ItemType.APPRENTICE)
                        {
                            freeApprenticesCount++;
                        }
                    }
                }
                

                if (freeAssistantsCount < 1)
                {
                    foreach (var building in Buildings)
                    {
                        if (building.ProduceItemType == ItemType.ASSISTANT && building.Level > 0)
                        {
                            var assistant = AssistantPools.Get(0);
                            Characters.Add(assistant);
                            assistant.transform.position = _workerSpawnPoint;
                            assistant.TargetBuilding = building;
                            assistant.FollowingOffset = 0.3f;
                            assistant.Initialized = true;
                        }
                    }
                }

                if (freeApprenticesCount < 1)
                {
                    foreach (var building in Buildings)
                    {
                        if (building.ProduceItemType == ItemType.APPRENTICE && building.Level > 0)
                        {
                            var apprentice = ApprenticePools.Get(0);
                            Characters.Add(apprentice);
                            apprentice.transform.position = _workerSpawnPoint;
                            apprentice.ResizeBagOfTries(apprentice.BaseBagOfTriesCapacity);
                            apprentice.TargetBuilding = building;
                            apprentice.FollowingOffset = 0.3f;
                            apprentice.Initialized = true;
                        }
                    }
                }
            }
        }
    }
}