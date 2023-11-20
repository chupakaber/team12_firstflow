using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class WorkerBehaviorSystem: ISystem
    {
        public List<Character> Characters;
        public List<Building> Buildings;

        public NavMeshPath _path;
        public Vector3[] _pathCorners = new Vector3[128];

        public void EventCatch(StartEvent newEvent)
        {
            _path = new NavMeshPath();
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.ASSISTANT || character.CharacterType == CharacterType.APPRENTICE)
                {
                    var worker = (Worker) character;
                    if (worker.TargetBuilding == null)
                    {
                        if (Time.time > worker.LastBehaviorTime + 0.2f)
                        {
                            worker.LastBehaviorTime = Time.time;
                            var targetPosition = worker.TargetCharacter != null ? worker.TargetCharacter.transform.position : worker.SpawnBuilding.PickingUpArea.transform.position;
                            var workerPosition = worker.transform.position;
                            if (worker.NavMeshAgent.CalculatePath(targetPosition, _path))
                            {
                                var pathPosition = targetPosition;
                                if (_path.GetCornersNonAlloc(_pathCorners) > 1)
                                {
                                    pathPosition = _pathCorners[1];
                                }
                                var pathDelta = pathPosition - workerPosition;
                                pathDelta.y = 0f;
                                character.WorldDirection = pathDelta.normalized;

                                var toTargetDistance = (targetPosition - workerPosition).magnitude;
                                
                                if (toTargetDistance < 1.5f) {
                                    character.WorldDirection = Vector3.zero;
                                }
                            }
                            else
                            {
                                character.WorldDirection = Vector3.zero;
                            }
                        }
                    }
                }
            }
        }
    }
}