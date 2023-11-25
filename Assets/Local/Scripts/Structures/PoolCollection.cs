using Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class PoolCollection<T> where T : PoolableObject
    {
        public Dictionary<ItemType, ObjectPool<T>> Pools = new Dictionary<ItemType, ObjectPool<T>>();

        public T Get(ItemType itemType)
        {
            if (Pools.TryGetValue(itemType, out var objectPool)) 
            { 
                return objectPool.Get();               
            }

            throw new System.Exception($"Pool {itemType} not found");
        }
    }
}
