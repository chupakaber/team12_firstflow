using UnityEngine;
using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine.UIElements;

namespace Scripts
{
    public class Character: Unit
    {
        [Header("Character Config")]
        public Rigidbody CharacterRigidbody;
        public Animator CharacterAnimator;
        public Transform MessageEmitterPivot;
        public float Speed;
        public CharacterType CharacterType;
        public int ItemLimit;
        public float PickUpCooldown;
        public float PickUpGoldMaxTime = 5f;
        public float DropGoldMaxTime = 5f;
        public ItemType PickUpItemConstraint = ItemType.NONE;
        public int BaseBagOfTriesCapacity = 8;
        public bool IsCutSceneActiv;

        [Header("Ranks Config (6, 5, 4, 3, 2, 1)")]
        public List<int> RankHonor = new List<int> {
            0,
            100,
            300,
            600,
            1000,
            1500,
            1000000
        };

        [Header("Character Runtime")]
        public Vector3 WorldDirection;
        public float LastDropItemTime;
        public float LastPickUpItemTime;
        public BagOfTries BagOfTries = new BagOfTries();
        public BagOfTriesView BagOfTriesView;
        public Stack<Collider> EnterColliders = new Stack<Collider>();
        public Stack<Collider> ExitColliders = new Stack<Collider>();
        public LinkedList<Character> CharacterCollisions = new LinkedList<Character>();
        public Character NextInQueue;
        public Character PreviousInQueue;


        private const float MIN_COOLDOWN = 0.06f;

        private int _loadedAnimationKey = Animator.StringToHash("Loaded");
        private int _speedAnimationKey = Animator.StringToHash("Speed");

        public void GetRank(out int rank, out int currentPoints, out int rankPoints)
        {
            var itemCount = Items.GetAmount(ItemType.HONOR);
            
            for (var i = 0; i < RankHonor.Count - 1; i++)
            {
                var rankHonor = RankHonor[i];
                var nextRankHonor = RankHonor[i + 1];
                if (itemCount < nextRankHonor)
                {
                    rank = 6 - i;
                    currentPoints = itemCount - rankHonor;
                    rankPoints = nextRankHonor - rankHonor;
                    return;
                }
            }

            rank = 6;
            currentPoints = 0;
            rankPoints = 0;
        }

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
        }

        public void AddLastInQueue(Character newCharacter)
        {
            if (newCharacter == this)
            {
                throw new UnityException("Trying to add in character queue itself.");
            }

            var character = this;
            while (character.NextInQueue != null)
            {
                character = character.NextInQueue;
            }

            character.NextInQueue = newCharacter;
            newCharacter.PreviousInQueue = character;
        }

        public void AddFirstInQueue(Character newCharacter)
        {
            var character = this;
            while (character.PreviousInQueue != null)
            {
                character = character.PreviousInQueue;
            }

            character.PreviousInQueue = newCharacter;
            newCharacter.NextInQueue = character;
        }

        public void LeaveQueue()
        {
            if (NextInQueue != null)
            {
                NextInQueue.PreviousInQueue = PreviousInQueue;
            }

            if (PreviousInQueue != null)
            {
                PreviousInQueue.NextInQueue = NextInQueue;
            }

            NextInQueue = null;
            PreviousInQueue = null;
        }

        public float GetPickUpCooldown(ItemType itemType, out int batchCount, int sourceCount = 1)
        {
            batchCount = 1;

            if (itemType == ItemType.GOLD)
            {
                var count = Mathf.Max(1, sourceCount);
                var cooldown = PickUpGoldMaxTime / count;
                cooldown = Mathf.Min(PickUpCooldown, cooldown);
                batchCount = (int) Mathf.Ceil(Mathf.Max(1f, MIN_COOLDOWN / cooldown));
                
                return cooldown;
            }

            return PickUpCooldown;
        }

        public float GetDropCooldown(ItemType itemType, out int batchCount, int capacity = 1)
        {
            batchCount = 1;

            if (itemType == ItemType.GOLD)
            {
                var count = Mathf.Max(1, capacity);
                var cooldown = DropGoldMaxTime / count;
                cooldown = Mathf.Min(PickUpCooldown, cooldown);
                batchCount = (int) Mathf.Ceil(Mathf.Max(1f, MIN_COOLDOWN / cooldown));

                return cooldown;
            }
            
            return PickUpCooldown;
        }
    }
}
