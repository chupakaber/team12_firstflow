using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class CustomerBehaviorSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public NavMeshPath _path;
        public Vector3[] _pathCorners = new Vector3[128];

        public void EventCatch(StartEvent newEvent)
        {
            _path = new NavMeshPath();
        }

        public void EventCatch(CustomerReachKeyEvent newEvent)
        {
            var customer = newEvent.Customer;
            customer.State++;

            if (customer.State == 3)
            {
                foreach (var item in customer.Items)
                {
                    customer.ItemStackView.RemoveItem(item.Type, item.Amount);
                }
                Characters.Remove(customer);
                Object.Destroy(customer.gameObject);
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var character in Characters)
            {
                if (character is Customer)
                {
                    var customer = (Customer) character;
                    var targetPosition = Vector3.zero;
                    
                    if (Time.time > customer.LastBehaviorTime + 0.1f)
                    {
                        customer.LastBehaviorTime = Time.time;
                        if (customer.State == 1)
                        {
                            if (customer.PreviousInQueue != null)
                            {
                                targetPosition = customer.PreviousInQueue.transform.position;
                                customer.FollowingOffset = 2.2f;
                            }
                            else
                            {
                                var waypoint = customer.Route.Waypoints[customer.CurrentRouteWaypointIndex];
                                targetPosition = waypoint.Transform.position;
                                customer.FollowingOffset = 0.3f;
                            }
                        }
                        else
                        {
                            if (customer.CurrentRouteWaypointIndex < customer.Route.Waypoints.Count)
                            {
                                var waypoint = customer.Route.Waypoints[customer.CurrentRouteWaypointIndex];
                                targetPosition = waypoint.Transform.position;
                                customer.FollowingOffset = 0.3f;

                                if ((customer.transform.position - targetPosition).magnitude < 0.5f)
                                {
                                    customer.CurrentRouteWaypointIndex++;

                                    if (waypoint.IsKeyPoint)
                                    {
                                        EventBus.CallEvent(new CustomerReachKeyEvent() { Customer = customer });
                                        break;
                                    }
                                }
                            }
                        }
                        
                        if (targetPosition.sqrMagnitude > float.Epsilon * 2f)
                        {
                            var workerPosition = customer.transform.position;

                            var toTargetDelta = targetPosition - workerPosition;
                            toTargetDelta.y = 0f;
                            var toTargetDistance = toTargetDelta.magnitude;

                            if (toTargetDistance < customer.FollowingOffset)
                            {
                                character.WorldDirection = Vector3.zero;
                            }
                            else
                            {
                                var pathPosition = targetPosition;

                                customer.NavMeshAgent.enabled = true;
                                if (customer.NavMeshAgent.CalculatePath(targetPosition, _path))
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
                                customer.NavMeshAgent.enabled = false;

                                var pathDelta = pathPosition - workerPosition;
                                // pathDelta.y = 0f;
                                character.WorldDirection = pathDelta.normalized * Mathf.Min(Mathf.Max(pathDelta.magnitude, 0.1f), 1f);
                            }
                        }
                    }
                }
            }
        }
    }
}