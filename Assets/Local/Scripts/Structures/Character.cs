using UnityEngine;
using System.Collections.Generic;
using Scripts.Enums;

namespace Scripts
{
    public class Character: Unit
    {
        [Header("Character Config")]
        public Rigidbody CharacterRigidbody;
        public Animator CharacterAnimator;
        public float Speed;
        public CharacterType CharacterType;
        public int ItemLimit;
        public float PickUpCooldown;
        public ItemType PickUpItemConstraint = ItemType.NONE;
        public int BaseBagOfTriesCapacity = 8;

        [Header("Player Config")]
        public float PlayerSpeedBoostAmount = 2f;

        [Header("Character Runtime")]
        public Vector3 WorldDirection;
        public float LastDropItemTime;
        public float LastPickUpItemTime;
        public BagOfTries BagOfTries = new BagOfTries();
        public BagOfTriesView BagOfTriesView;
        public Stack<Collider> EnterColliders = new Stack<Collider>();
        public Stack<Collider> ExitColliders = new Stack<Collider>();
        public LinkedList<Character> CharacterCollisions = new LinkedList<Character>();

        private int _loadedAnimationKey = Animator.StringToHash("Loaded");
        private int _speedAnimationKey = Animator.StringToHash("Speed");

        public void OnTriggerEnter(Collider other)
        {
            var character = other.gameObject.GetComponent<Character>();
            if (character != null)
            {
                if (!CharacterCollisions.Contains(character))
                {
                    CharacterCollisions.AddLast(character);
                }
            }
            EnterColliders.Push(other);
        }

        public void OnTriggerExit(Collider other)
        {
            ExitColliders.Push(other);
        }

        public void OnCollisionEnter(Collision collision)
        {
            var character = collision.collider.gameObject.GetComponent<Character>();
            if (character != null)
            {
                if (!CharacterCollisions.Contains(character))
                {
                    CharacterCollisions.AddLast(character);
                }
            }
        }

        public bool GetActionTry()
        {
            if (BagOfTries.TryGetNext(out var lastIndex, out var changedValue, out var nextIndex, out var nextValue))
            {
                BagOfTriesView.Roll(nextIndex, 0.4f);
                BagOfTriesView.SetValue(lastIndex, changedValue);
                return nextValue;
            }

            return false;
        }

        public void RestoreActionTries()
        {
            BagOfTries.Restore();

            if (BagOfTriesView != null)
            {
                for (var i = 0; i < BagOfTries.Values.Count; i++)
                {
                    BagOfTriesView.SetValue(i, BagOfTries.Values[i]);
                }
            }
        }

        public void ResizeBagOfTries(int size)
        {
            BagOfTries.Resize(size);
        }

        public void ShowBagOfTries()
        {
            if (BagOfTriesView != null)
            {
                BagOfTriesView.Show();
            }
        }

        public void HideBagOfTries()
        {
            if (BagOfTriesView != null)
            {
                BagOfTriesView.Hide();
            }
        }

        public void UpdateAnimation()
        {
            CharacterAnimator.SetBool(_loadedAnimationKey, Items.GetAmount() > 0);
            CharacterAnimator.SetFloat(_speedAnimationKey, (CharacterRigidbody.velocity.magnitude - 0.5f) / 4f);
        }

        public virtual void LevelUp()
        {
            if (CharacterType == CharacterType.PLAYER)
            {
                Speed += PlayerSpeedBoostAmount;
            }
        }
    }
}
