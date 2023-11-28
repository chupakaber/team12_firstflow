using UnityEngine;
using System.Collections.Generic;
using Scripts.Enums;

namespace Scripts
{
    public class Character: Unit
    {
        [Header("Character Config")]
        public Rigidbody CharacterRigidbody;
        public float Speed;
        public Stack<Collider> EnterColliders = new Stack<Collider>();
        public Stack<Collider> ExitColliders = new Stack<Collider>();
        public CharacterType CharacterType;
        public int ItemLimit;
        public float PickUpCooldown;
        public ItemType PickUpItemConstraint = ItemType.NONE;

        [Header("Character Runtime")]
        public Vector3 WorldDirection;
        public float LastDropItemTime;
        public float LastPickUpItemTime;

        public void OnTriggerEnter(Collider other)
        {
            EnterColliders.Push(other);
        }

        public void OnTriggerExit(Collider other)
        {
            ExitColliders.Push(other);
        }
    }
}
