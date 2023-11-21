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
                if (character is Worker)
                {
                    var worker = (Worker) character;
                    if (Time.time > worker.LastBehaviorTime + 0.1f)
                    {
                        worker.LastBehaviorTime = Time.time;

                        if (worker.TargetCharacter != null)
                        {
                            worker.TargetPosition = worker.TargetCharacter.transform.position;
                        }
                        else if (worker.TargetBuilding != null)
                        {
                            if (worker.TargetBuilding.ProduceItemType == ItemType.APPRENTICE || worker.TargetBuilding.ProduceItemType == ItemType.ASSISTANT)
                            {
                                worker.TargetPosition = worker.TargetBuilding.PickingUpArea.transform.position;
                            }
                        }

                        
                        if (worker.TargetPosition.sqrMagnitude > float.Epsilon * 2f)
                        {
                            var workerPosition = worker.transform.position;

                            var toTargetDistance = (worker.TargetPosition - workerPosition).magnitude;
                            if (toTargetDistance < worker.FollowingOffset)
                            {
                                character.WorldDirection = Vector3.zero;
                            }
                            else
                            {
                                var pathPosition = worker.TargetPosition;

                                if (worker.NavMeshAgent.CalculatePath(worker.TargetPosition, _path))
                                {
                                    var cornersCount = _path.GetCornersNonAlloc(_pathCorners);
                                    if (cornersCount > 1)
                                    {
                                        pathPosition = _pathCorners[1];
                                    }
                                    else if (cornersCount > 0)
                                    {
                                        pathPosition = _pathCorners[0];
                                    }
                                }

                                var pathDelta = pathPosition - workerPosition;
                                pathDelta.y = 0f;
                                character.WorldDirection = pathDelta.normalized;
                            }
                        }
                    }
                }
            }
        }
    }
}