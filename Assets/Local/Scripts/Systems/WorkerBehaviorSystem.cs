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
                        if (worker.PreviousInQueue != null)
                        {
                            worker.TargetPosition = worker.PreviousInQueue.transform.position;
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

                            var toTargetDelta = worker.TargetPosition - workerPosition;
                            toTargetDelta.y = 0f;
                            var toTargetDistance = toTargetDelta.magnitude;

                            if (toTargetDistance < worker.FollowingOffset)
                            {
                                character.WorldDirection = Vector3.zero;
                            }
                            else
                            {
                                var pathPosition = worker.TargetPosition;

                                worker.NavMeshAgent.enabled = true;
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
                                worker.NavMeshAgent.enabled = false;

                                var pathDelta = pathPosition - workerPosition;
                                // pathDelta.y = 0f;
                                character.WorldDirection = pathDelta.normalized * Mathf.Min(Mathf.Max(pathDelta.magnitude, 0.2f), 1f);

                                if (Physics.Raycast(character.transform.position + Vector3.up * 1.7f, character.WorldDirection, out var hitInfo, 2f))
                                {
                                    if (hitInfo.collider is CapsuleCollider)
                                    {
                                        character.WorldDirection = Quaternion.Euler(0f, 45f, 0f) * character.WorldDirection;
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