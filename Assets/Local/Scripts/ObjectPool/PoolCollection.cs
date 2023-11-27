using System.Collections.Generic;

namespace Scripts
{
    public class PoolCollection<T> where T : PoolableObject
    {
        public Dictionary<int, ObjectPool<T>> Pools = new Dictionary<int, ObjectPool<T>>();

        public T Get(int itemType)
        {
            if (Pools.TryGetValue(itemType, out var objectPool)) 
            { 
                return objectPool.Get();               
            }

            throw new System.Exception($"Pool {itemType} not found");
        }
    }
}
