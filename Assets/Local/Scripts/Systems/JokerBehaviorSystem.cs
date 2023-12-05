using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class JokerBehaviorSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public PoolCollection<Joker> JokerPools;

        public Joker _joker;
        public NavMeshPath _path;
        public Vector3[] _pathCorners = new Vector3[128];

        public void EventCatch(StartEvent newEvent)
        {
            _path = new NavMeshPath();
            _joker = JokerPools.Get(0);
            Characters.Add(_joker);
            var routes = Object.FindObjectsOfType<CustomerRoute>();
            _joker.transform.position = routes[0].Waypoints[0].Transform.position;
            _joker.SpawnPoint = _joker.transform.position;
            _joker.transform.localScale = Vector3.one;
        }

/*
        public void EventCatch(FixedUpdateEvent newEvent)
        {
            if (Time.time > _joker.LastBehaviorTime + 0.2f)
            {
                _joker.LastBehaviorTime = Time.time;

                var targetPosition = _joker.transform.position;

                switch (_joker.State)
                {
                    case 0:
                    var activeBuildingCount = 0;
                    foreach (var building in Buildings)
                    {
                        if (building.Level > 0)
                        {
                            activeBuildingCount++;
                        }
                    }
                    var randomIndex = (int) Mathf.Floor(Random.Range(0f, activeBuildingCount));
                    var i = 0;
                    foreach (var building in Buildings)
                    {
                        if (building.Level > 0)
                        {
                            if (i == randomIndex)
                            {
                                _joker.TargetBuilding = building;
                                _joker.State = 1;
                                _joker.LastBehaviorTime = Time.time + 10f;
                                break;
                            }
                            i++;
                        }
                    }
                    break;

                    case 1:
                    if (_joker.TargetBuilding != null)
                    {
                        targetPosition = _joker.TargetBuilding.transform.position;
                    }
                    break;
                }

                if (targetPosition.sqrMagnitude > float.Epsilon * 2f)
                {
                    var jokerPosition = _joker.transform.position;

                    var toTargetDelta = targetPosition - jokerPosition;
                    toTargetDelta.y = 0f;
                    var toTargetDistance = toTargetDelta.magnitude;

                    if (toTargetDistance < _joker.FollowingOffset)
                    {
                        _joker.WorldDirection = Vector3.zero;
                    }
                    else
                    {
                        var pathPosition = targetPosition;

                        _joker.NavMeshAgent.enabled = true;
                        if (_joker.NavMeshAgent.CalculatePath(targetPosition, _path))
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
                        _joker.NavMeshAgent.enabled = false;

                        var pathDelta = pathPosition - jokerPosition;
                        // pathDelta.y = 0f;
                        _joker.WorldDirection = pathDelta.normalized * Mathf.Min(Mathf.Max(pathDelta.magnitude, 0.1f), 1f);

                        if (Physics.Raycast(_joker.transform.position + Vector3.up * 1.7f, _joker.WorldDirection, out var hitInfo, 2f))
                        {
                            if (hitInfo.collider is CapsuleCollider)
                            {
                                _joker.WorldDirection = Quaternion.Euler(0f, 45f, 0f) * _joker.WorldDirection;
                            }
                        }
                    }
                }
            }
        }
*/
    }
}