using UnityEngine;
using System.Collections.Generic;

namespace Scripts
{
    public class Character: Unit
    {
        public Rigidbody CharacterRigidbody;
        public float Speed;
        public Stack<Collider> EnterColliders = new Stack<Collider>();
        public Stack<Collider> ExitColliders = new Stack<Collider>();
        public bool IsPlayer;
        public int ItemLimit;
        public float PickUpCooldown;

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
