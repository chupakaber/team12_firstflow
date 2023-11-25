using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class ObjectPool<T>: IObjectPool where T : PoolableObject
    {
        public T itemPrefab;
        private Stack<PoolableObject> Objects = new Stack<PoolableObject>();

        public ObjectPool(string prefabPath)
        {
            itemPrefab = Resources.Load<T>(prefabPath);
        }

        public T Get() 
        {
            if (Objects.TryPop(out PoolableObject o)) 
            { 
                o.Activate();
                return (T)o;
            }
            var newObject = Object.Instantiate(itemPrefab);
            newObject.Pool = this;
            return newObject;
        }

        public void Add(PoolableObject o) 
        {
            Objects.Push(o);
        }
    }
}
