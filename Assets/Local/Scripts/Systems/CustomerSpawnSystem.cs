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
        // private Player _player;

        public void EventCatch(InitEvent newEvent)
        {
            var routes = Object.FindObjectsOfType<CustomerRoute>();
            foreach (var route in routes)
            {
                _customerRoutes.TryAdd(route.ProductionType, route);
            }

            // foreach (var character in Characters)
            // {
            //     if (character is Player)
            //     {
            //         _player = (Player) character;
            //     }
            // }
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
                                var random = Random.Range(0f, 1f);
                                var randomIndex = random < 0.2f ? 1 : (random > 0.8f ? 2 : 0);

                                var customer = CustomerPools.Get(randomIndex);

                                // _player.GetRank(out var playerRank, out _, out _);
                                // if (customer.Rank < playerRank)
                                // {
                                //     customer.Release();
                                //     customer = CustomerPools.Get(0);
                                // }

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