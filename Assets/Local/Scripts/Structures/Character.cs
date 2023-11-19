using UnityEngine;
using System.Collections.Generic;
using Scripts.Enums;

namespace Scripts
{
    public class Character: Unit
    {
        public Rigidbody CharacterRigidbody;
        public Vector3 WorldDirection;
        public float Speed;
        public float LastMoveItemTime;
        public Stack<Collider> EnterColliders = new Stack<Collider>();
        public Stack<Collider> ExitColliders = new Stack<Collider>();
        public bool IsPlayer;
        public int ItemLimit;

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
