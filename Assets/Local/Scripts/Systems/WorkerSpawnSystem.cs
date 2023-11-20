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
                        if (worker.TargetBuilding == null)
                        {
                            freeAssistantsCount++;
                        }
                    }

                    if (character.CharacterType == CharacterType.APPRENTICE)
                    {
                        var worker = (Worker) character;
                        if (worker.TargetBuilding == null)
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

                    character.transform.position = Vector3.zero;

                    var assistant = (Assistant) character;
                    foreach (var building in Buildings)
                    {
                        if (building.ProduceItemType == ItemType.ASSISTANT)
                        {
                            assistant.SpawnBuilding = building;
                        }
                    }
                }

                // TODO: add spawning apprentice
            }
        }
    }
}