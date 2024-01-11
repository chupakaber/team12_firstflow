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

        public virtual void Activate()
        {
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
