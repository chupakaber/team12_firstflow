using UnityEngine;
using System.Collections.Generic;
using Scripts.Enums;

namespace Scripts
{
    public class Character: MonoBehaviour
    {
        public Rigidbody CharacterRigidbody;
        public Vector3 WorldDirection;
        public float Speed;
        public Collider CollidedWith;
        [SerializeField] public Dictionary<ItemType, int> Items = new Dictionary<ItemType, int>();

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

        public void AddItem(ItemType type, int count)
        {
            if (!Items.TryAdd(type, count))
            {
                Items[type] += count;
            }
                Debug.Log($"Кол-во {type} равно {Items[type]}");
            
        }
    }
}
