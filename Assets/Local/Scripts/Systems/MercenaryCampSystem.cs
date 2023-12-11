using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class MercenaryCampSystem: ISystem
    {
        public List<Character> Characters;
        public List<Building> Buildings;

        public PoolCollection<Mercenary> MercenaryPools;

        private NavMeshPath _path;
        private Vector3[] _pathCorners = new Vector3[128];
        private CustomerRoute _mercenaryRoute;

        private const int MERCENARY_LIMIT = 6;

        public void EventCatch(StartEvent newEvent)
        {
            _path = new NavMeshPath();

            var routes = GameObject.FindObjectsOfType<CustomerRoute>();
            foreach (var route in routes)
            {
                if (route.ProductionType == ItemType.MERCENARY)
                {
                    _mercenaryRoute = route;
                    break;
                }
            }

            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.MERCENARY)
                {
                    var mercenary = (Mercenary) character;
                    mercenary.Route = _mercenaryRoute;
                }
            }
        }

        public void EventCatch(AddItemEvent newEvent)
        {
            if (newEvent.ItemType == ItemType.GOLD && newEvent.Unit is Building)
            {
                var building = (Building) newEvent.Unit;
                if (building.ConsumeItemType == ItemType.NONE)
                {
                    EndRaid();
                }
            }
        }

        public void EventCatch(RemoveItemEvent newEvent)
        {
            if (newEvent.ItemType == ItemType.GOLD && newEvent.Unit is Building)
            {
                var building = (Building) newEvent.Unit;
                if (building.ConsumeItemType == ItemType.NONE)
                {
                    if (building.Items.GetAmount(building.ProduceItemType) < 1)
                    {
                        StartRaid();
                    }
                }
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (building.ConsumeItemType == ItemType.NONE && building.ProduceItemType == ItemType.GOLD)
                {
                    foreach (var character in building.UnloadingCharacters)
                    {
                        if (character.CharacterType == CharacterType.PLAYER)
                        {
                            if (MercenaryCreate(building, character))
                            {
                                return;
                            }
                        }
                    }
                }
            }
            
            foreach (var character in Characters)
            {
                if (character is Mercenary)
                {
                    var mercenary = (Mercenary) character;
                    var targetPosition = Vector3.zero;
                    
                    if (Time.time > mercenary.LastMercenaryBehaviorTime + 0.5f)
                    {
                        mercenary.LastMercenaryBehaviorTime = Time.time;
                        if (mercenary.PreviousInQueue != null)
                        {
                            targetPosition = mercenary.PreviousInQueue.transform.position;
                            mercenary.FollowingOffset = 2.2f;
                        }
                        else
                        {
                            if (mercenary.State == 0 || mercenary.State == 2)
                            {
                                if (mercenary.CurrentRouteWaypointIndex < mercenary.Route.Waypoints.Count)
                                {
                                    var waypoint = mercenary.Route.Waypoints[mercenary.CurrentRouteWaypointIndex];
                                    targetPosition = waypoint.Transform.position;
                                    mercenary.FollowingOffset = 0.3f;
                                }
                            }
                            else
                            {
                                if (mercenary.CurrentRouteWaypointIndex < mercenary.Route.Waypoints.Count)
                                {
                                    var waypoint = mercenary.Route.Waypoints[mercenary.CurrentRouteWaypointIndex];
                                    targetPosition = waypoint.Transform.position;
                                    mercenary.FollowingOffset = 0.3f;

                                    if ((mercenary.transform.position - targetPosition).magnitude < mercenary.FollowingOffset + 0.2f)
                                    {
                                        mercenary.CurrentRouteWaypointIndex++;

                                        if (waypoint.IsKeyPoint)
                                        {
                                            mercenary.State++;
                                            if (mercenary.State > 3)
                                            {
                                                mercenary.State = 0;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        
                        if (targetPosition.sqrMagnitude > float.Epsilon * 2f)
                        {
                            var workerPosition = mercenary.transform.position;

                            var toTargetDelta = targetPosition - workerPosition;
                            toTargetDelta.y = 0f;
                            var toTargetDistance = toTargetDelta.magnitude;

                            if (toTargetDistance < mercenary.FollowingOffset)
                            {
                                character.WorldDirection = Vector3.zero;
                            }
                            else
                            {
                                var pathPosition = targetPosition;

                                mercenary.NavMeshAgent.enabled = true;
                                if (mercenary.NavMeshAgent.CalculatePath(targetPosition, _path))
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
                                mercenary.NavMeshAgent.enabled = false;

                                var pathDelta = pathPosition - workerPosition;
                                // pathDelta.y = 0f;
                                character.WorldDirection = pathDelta.normalized * Mathf.Min(Mathf.Max(pathDelta.magnitude, 0.1f), 1f);

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

        public bool MercenaryCreate(Building building, Character character)
        {            
            var mercenaryCount = 0;
            foreach (var otherCharacter in Characters)
            {
                if (otherCharacter.CharacterType == CharacterType.MERCENARY)
                {
                    mercenaryCount++;
                }
            }

            if (mercenaryCount >= MERCENARY_LIMIT)
            {
                return false;
            }

            if (character.NextInQueue != null)
            {
                var recrut = character.NextInQueue;

                var mercenary = MercenaryPools.Get(0);
                Characters.Add(mercenary);
                Characters.Remove(recrut);
                mercenary.transform.position = recrut.transform.position;

                building.ProductionCharacters.Remove(recrut);
                building.UnloadingCharacters.Remove(recrut);
                recrut.LeaveQueue();
                recrut.Release();

                building.ProductionItemAmountPerCycle += mercenary.ProductionAmount;
                building.ProductionLimit = building.ProductionItemAmountPerCycle;

                mercenary.Route = _mercenaryRoute;

                return true;
            }
            return false;
        }

        public void StartRaid() 
        {
            Mercenary firstMercenary = null;
            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.MERCENARY)
                {
                    var mercenary = (Mercenary) character;

                    if (mercenary.Route == null)
                    {
                        mercenary.Route = _mercenaryRoute;
                    }

                    if (firstMercenary == null)
                    {
                        firstMercenary = mercenary;
                    }
                    else
                    {
                        firstMercenary.AddLastInQueue(mercenary);
                    }
                    mercenary.State = 1;
                    mercenary.CurrentRouteWaypointIndex = 0;
                    mercenary.FollowingOffset = 2.2f;
                }
            }
        }

        public void EndRaid() 
        {
            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.MERCENARY)
                {
                    var mercenary = (Mercenary) character;

                    if (mercenary.Route == null)
                    {
                        mercenary.Route = _mercenaryRoute;
                    }

                    mercenary.LeaveQueue();
                    mercenary.State = 3;
                }
            }
        }
    }
}
