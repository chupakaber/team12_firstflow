using UnityEngine;

namespace Scripts
{
    public class PoolableObject: MonoBehaviour
    {
        public IObjectPool Pool;

        public void Release()
        {
            Pool.Add(this);
            gameObject.SetActive(false);
            transform.SetParent(null);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }
    }
}
