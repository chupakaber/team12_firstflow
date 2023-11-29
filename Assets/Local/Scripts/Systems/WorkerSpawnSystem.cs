using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class WorkerSpawnSystem: ISystem
    {
        public List<Character> Characters;
        public List<Building> Buildings;

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
                    var prefab = Resources.Load<GameObject>("Prefabs/Characters/Assistant");
                    var instance = Object.Instantiate(prefab);
                    var character = instance.GetComponent<Character>();
                    Characters.Add(character);

                    character.transform.position = _workerSpawnPoint;

                    var assistant = (Assistant) character;
                    foreach (var building in Buildings)
                    {
                        if (building.ProduceItemType == ItemType.ASSISTANT)
                        {
                            assistant.TargetBuilding = building;
                            assistant.FollowingOffset = 0.3f;
                        }
                    }
                }

                if (freeApprenticesCount < 1)
                {
                    var prefab = Resources.Load<GameObject>("Prefabs/Characters/Apprentice");
                    var instance = Object.Instantiate(prefab);
                    var character = instance.GetComponent<Character>();
                    Characters.Add(character);

                    character.transform.position = _workerSpawnPoint;
                    character.ResizeBagOfTries(character.BaseBagOfTriesCapacity);

                    var apprentice = (Apprentice) character;
                    foreach (var building in Buildings)
                    {
                        if (building.ProduceItemType == ItemType.APPRENTICE)
                        {
                            apprentice.TargetBuilding = building;
                            apprentice.FollowingOffset = 0.3f;
                        }
                    }
                }
            }
        }
    }
}