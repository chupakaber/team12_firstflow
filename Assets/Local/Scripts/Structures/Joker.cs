using UnityEngine;
using Scripts.BehaviorTree;

namespace Scripts
{
    public class Joker: SmartCharacter
    {
        [Header("Joker Config")]
        public float Action1Chance = 0.5f;

        [Header("Joker Runtime")]
        public int State;
    }
}