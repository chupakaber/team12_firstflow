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
        public ItemStack Items = new ItemStack();
        public ItemStackView ItemStackView;
        public float LastMoveItemTime;

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
            Items.AddItem(type, count);

            foreach (var item in Items) 
            {
                Debug.Log($"Кол-во {item.Type} равно {item.Amount}");
            }           
        }

        public void RemoveItem(ItemType type, int count) 
        {
            {
                Items.RemoveItem(type, count);
            }
        }
    }
}
