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

        [Header("Character Runtime")]
        public Vector3 WorldDirection;
        public float LastMoveItemTime;

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log($"{other.name}Enter");
            EnterColliders.Push( other );
        }

        public void OnTriggerExit(Collider other)
        {
            Debug.Log($"{other.name}Exit");
            ExitColliders.Push( other );            
        }
    }
}
