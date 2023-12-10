using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class CarnivalBehaviorSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public NavMeshPath _path;
        public Vector3[] _pathCorners = new Vector3[128];

        public void EventCatch(StartEvent newEvent)
        {
            _path = new NavMeshPath();

            var carnival = GameObject.FindObjectsOfType<Carnival>();
            foreach (var character in carnival)
            {
                Characters.Add(character);

                EventBus.CallEvent(new AddItemEvent() { Unit = character, Count = 1, ItemType = Enums.ItemType.WOOD });
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var character in Characters)
            {
                if (character is Carnival)
                {
                    var carnival = (Carnival) character;
                    var targetPosition = Vector3.zero;
                    
                    if (Time.time > carnival.LastBehaviorTime + 0.1f)
                    {
                        carnival.LastBehaviorTime = Time.time;
                        if (carnival.State == 1)
                        {
                            if (carnival.PreviousInQueue != null)
                            {
                                targetPosition = carnival.PreviousInQueue.transform.position;
                                //carnival.FollowingOffset = 2.2f;
                            }
                            else
                            {
                                var waypoint = carnival.Route.Waypoints[carnival.CurrentRouteWaypointIndex];
                                targetPosition = waypoint.Transform.position;
                                //carnival.FollowingOffset = 0.3f;
                            }
                        }
                        else if (carnival.State == 0)
                        {
                            if (carnival.CurrentRouteWaypointIndex < carnival.Route.Waypoints.Count)
                            {
                                var waypoint = carnival.Route.Waypoints[carnival.CurrentRouteWaypointIndex];
                                targetPosition = waypoint.Transform.position;
                                //carnival.FollowingOffset = 0.3f;

                                if ((carnival.transform.position - targetPosition).magnitude < 0.5f)
                                {
                                    carnival.CurrentRouteWaypointIndex++;

                                    if (waypoint.IsKeyPoint)
                                    {
                                        carnival.CurrentRouteWaypointIndex = 0;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            carnival.CurrentRouteWaypointIndex = 0;
                            var waypoint = carnival.Route.Waypoints[carnival.CurrentRouteWaypointIndex];
                            targetPosition = waypoint.Transform.position;
                        }

                        if (carnival.NextInQueue != null)
                        {
                            if ((carnival.NextInQueue.transform.position - carnival.transform.position).magnitude > 3.5f)
                            {
                                targetPosition = carnival.transform.position;
                                carnival.WorldDirection = Vector3.zero;
                            }
                        }
                        
                        if (targetPosition.sqrMagnitude > float.Epsilon * 2f)
                        {
                            var workerPosition = carnival.transform.position;

                            var toTargetDelta = targetPosition - workerPosition;
                            toTargetDelta.y = 0f;
                            var toTargetDistance = toTargetDelta.magnitude;

                            if (toTargetDistance < carnival.FollowingOffset)
                            {
                                character.WorldDirection = Vector3.zero;
                            }
                            else
                            {
                                var pathPosition = targetPosition;

                                carnival.NavMeshAgent.enabled = true;
                                if (carnival.NavMeshAgent.CalculatePath(targetPosition, _path))
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
                                carnival.NavMeshAgent.enabled = false;

                                var pathDelta = pathPosition - workerPosition;
                                // pathDelta.y = 0f;
                                character.WorldDirection = pathDelta.normalized * Mathf.Min(Mathf.Max(pathDelta.magnitude, 0.1f), 1f);

                                if (Physics.Raycast(character.transform.position + Vector3.up * 1.7f, character.WorldDirection, out var hitInfo, 2f))
                                {
                                    if (hitInfo.collider is CapsuleCollider && hitInfo.collider.name.CompareTo("Carnival") != 0)
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