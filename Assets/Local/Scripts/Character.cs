using UnityEngine;

namespace Scripts
{
    public class Character: MonoBehaviour
    {
        public Rigidbody CharacterRigidbody;
        public Vector3 WorldDirection;
        public float Speed;
        public Collider CollidedWith;

        public void OnTriggerEnter(Collider other)
        {
            CollidedWith = other;
            Debug.Log($"{other.name}Enter");
        }

        public void OnTriggerExit(Collider other)
        {
            if (other == CollidedWith)
            {
                CollidedWith = null;
                Debug.Log($"{other.name}Exit");
            }
        }
    }
}
