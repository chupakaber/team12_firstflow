using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class DaosDonkeySystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        private NavMeshPath _path;
        private Vector3[] _pathCorners = new Vector3[128];
        private CustomerRoute _donkeyRoute;
        private SmartCharacter _donkey;
        private Building _daosBuilding;
        private float _lastBehaviorTime;
        private bool _move = true;
        private bool _cleaning = false;

        private const int DONKEY_LOAD_LIMIT = 30;

        public void EventCatch(StartEvent newEvent)
        {
            _path = new NavMeshPath();

            var routes = GameObject.FindObjectsOfType<CustomerRoute>();
            foreach (var route in routes)
            {
                if (route.ProductionType == ItemType.HONOR)
                {
                    _donkeyRoute = route;
                    break;
                }
            }

            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.DONKEY)
                {
                    _donkey = (SmartCharacter) character;
                    _donkey.Route = _donkeyRoute;
                    _donkey.CurrentRouteWaypointIndex = _donkeyRoute.Waypoints.Count - 1;
                }
            }

            foreach (var building in Buildings)
            {
                if (building.ConsumeItemType == ItemType.ALL_PHYSIC_NON_UNIQUE)
                {
                    _daosBuilding = building;
                }
            }
        }

        public void EventCatch(AddItemEvent newEvent)
        {
            if (newEvent.Unit == _donkey)
            {
                if (!_move && _donkey.Items.GetAmount() >= 15 && _daosBuilding.Items.GetAmount() <= 0)
                {
                    _move = true;
                    _donkey.CurrentRouteWaypointIndex = 1;
                }
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            var targetPosition = Vector3.zero;

            if (!_cleaning && !_move && Time.time > _lastBehaviorTime + 0.06f && _daosBuilding.Items.GetAmount() >= 0)
            {
                _lastBehaviorTime = Time.time;

                Item movingItem = null;
                foreach (var item in _daosBuilding.Items)
                {
                    if (item.Type != ItemType.HONOR && item.Amount > 0)
                    {
                        movingItem = item;
                    }
                }

                if (movingItem != null)
                {
                    var sourcePileTopPosition = _daosBuilding.GetStackTopPosition(movingItem.Type);
                    var removeItemEvent = new RemoveItemEvent() { ItemType = movingItem.Type, Count = 1, Unit = _daosBuilding };
                    var addItemEvent = new AddItemEvent() { ItemType = movingItem.Type, Count = 1, Unit = _donkey, FromPosition = sourcePileTopPosition };
                    EventBus.CallEvent(removeItemEvent);
                    EventBus.CallEvent(addItemEvent);
                }
            }

            if (_cleaning && Time.time > _lastBehaviorTime + 0.06f)
            {
                _lastBehaviorTime = Time.time;
                if (_donkey.Items.GetAmount() > 0)
                {
                    Item itemToRemove = null;
                    foreach (var item in _donkey.Items)
                    {
                        if (item.Amount > 0)
                        {
                            itemToRemove = item;
                            break;
                        }
                    }
                    if (itemToRemove != null)
                    {
                        EventBus.CallEvent(new RemoveItemEvent() { Unit = _donkey, ItemType = itemToRemove.Type, Count = 1});
                    }
                }
                else
                {
                    _cleaning = false;
                }
            }
            
            if (Time.time > _lastBehaviorTime + 0.5f)
            {
                _lastBehaviorTime = Time.time;
                if (!_move)
                {
                    return;
                }

                if (_donkey.PreviousInQueue != null)
                {
                    targetPosition = _donkey.PreviousInQueue.transform.position;
                    _donkey.CurrentRouteWaypointIndex = ((Mercenary)_donkey.PreviousInQueue).CurrentRouteWaypointIndex;
                    _donkey.FollowingOffset = 2.2f;
                }
                else
                {
                    var waypoint = _donkey.Route.Waypoints[_donkey.CurrentRouteWaypointIndex % _donkey.Route.Waypoints.Count];
                    targetPosition = waypoint.Transform.position;
                    _donkey.FollowingOffset = 0.3f;

                    if ((_donkey.transform.position - targetPosition).magnitude < _donkey.FollowingOffset + 0.2f)
                    {
                        _donkey.CurrentRouteWaypointIndex++;

                        if (waypoint.IsKeyPoint)
                        {
                            if (_donkey.CurrentRouteWaypointIndex >= _donkeyRoute.Waypoints.Count || _donkey.CurrentRouteWaypointIndex == 0)
                            {
                                Debug.Log($"HOME");
                                _donkey.CurrentRouteWaypointIndex = 0;
                                _move = false;
                            }
                            else
                            {
                                _cleaning = true;
                                Debug.Log($"CLEAR DONKEY BASKETS");
                            }
                        }
                    }
                }
                
                if (targetPosition.sqrMagnitude > float.Epsilon * 2f)
                {
                    var workerPosition = _donkey.transform.position;

                    var toTargetDelta = targetPosition - workerPosition;
                    toTargetDelta.y = 0f;
                    var toTargetDistance = toTargetDelta.magnitude;

                    if (toTargetDistance < _donkey.FollowingOffset)
                    {
                        _donkey.WorldDirection = Vector3.zero;
                    }
                    else
                    {
                        var pathPosition = targetPosition;

                        _donkey.NavMeshAgent.enabled = true;
                        if (_donkey.NavMeshAgent.CalculatePath(targetPosition, _path))
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
                        _donkey.NavMeshAgent.enabled = false;

                        var pathDelta = pathPosition - workerPosition;
                        // pathDelta.y = 0f;
                        _donkey.WorldDirection = pathDelta.normalized * Mathf.Min(Mathf.Max(pathDelta.magnitude, 0.1f), 1f);

                        if (Physics.Raycast(_donkey.transform.position + Vector3.up * 1.7f, _donkey.WorldDirection, out var hitInfo, 2f))
                        {
                            if (hitInfo.collider is CapsuleCollider)
                            {
                                _donkey.WorldDirection = Quaternion.Euler(0f, 45f, 0f) * _donkey.WorldDirection;
                            }
                        }
                    }
                }
                else
                {
                    _donkey.WorldDirection = Vector3.zero;
                }
            }
        }
    }
}
