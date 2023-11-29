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
        public int BaseBagOfTriesCapacity = 8;

        [Header("Character Runtime")]
        public Vector3 WorldDirection;
        public float LastDropItemTime;
        public float LastPickUpItemTime;
        public BagOfTries BagOfTries = new BagOfTries();
        public BagOfTriesView BagOfTriesView;

        public void OnTriggerEnter(Collider other)
        {
            EnterColliders.Push(other);
        }

        public void OnTriggerExit(Collider other)
        {
            ExitColliders.Push(other);
        }

        public bool GetActionTry()
        {
            if (BagOfTries.TryGetNext(out var lastIndex, out var changedValue, out var nextIndex, out var nextValue))
            {
                BagOfTriesView.Roll(nextIndex, 0.2f);
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
    }
}
