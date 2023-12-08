using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class CustomerSpawnSystem: ISystem
    {
        public List<Character> Characters;
        public List<Building> Buildings;

        public PoolCollection<Customer> CustomerPools;

        private float _lastCheckTime = -3f;
        private Dictionary<ItemType, CustomerRoute> _customerRoutes = new Dictionary<ItemType, CustomerRoute>();

        public void EventCatch(StartEvent newEvent)
        {
            var routes = Object.FindObjectsOfType<CustomerRoute>();
            foreach (var route in routes)
            {
                _customerRoutes.TryAdd(route.ProductionType, route);
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            if (Time.time > _lastCheckTime + 0.3f) {
                _lastCheckTime = Time.time;

                foreach (var building in Buildings)
                {
                    if (building.ProduceItemType == ItemType.GOLD && building.ProductionArea != null && building.Level > 0)
                    {
                        if (_customerRoutes.TryGetValue(building.ConsumeItemType, out var route))
                        {
                            var customersCount = 0;
                            foreach (var character in Characters)
                            {
                                if (character.CharacterType == CharacterType.CUSTOMER)
                                {
                                    var customer = (Customer) character;
                                    if (customer.TargetBuilding == building)
                                    {
                                        customersCount++;
                                    }
                                }
                            }

                            if (customersCount < 5)
                            {
                                var customer = CustomerPools.Get(Random.Range(0f, 1f) > 0.3f ? 0 : 1);
                                Characters.Add(customer);

                                customer.Route = route;
                                customer.transform.position = customer.Route.Waypoints[0].Transform.position;
                                customer.TargetBuilding = building;
                                foreach (var otherCharacter in Characters)
                                {
                                    if (otherCharacter.CharacterType == CharacterType.CUSTOMER)
                                    {
                                        var otherCustomer = (Customer) otherCharacter;
                                        if (otherCustomer != customer && otherCustomer.TargetBuilding == customer.TargetBuilding)
                                        {
                                            otherCustomer.AddLastInQueue(customer);
                                            break;
                                        }
                                    }
                                }

                                customer.FollowingOffset = 2.2f;
                            }
                        }
                    }
                }
            }
        }
    }
}