using UnityEngine;

namespace Scripts
{
    public class Customer: SmartCharacter
    {
        [Header("Customer Config")]
        public int Rank = 6;

        [Header("Customer Runtime")]
        public Building TargetBuilding;
        public float FollowingOffset = 2.2f;
        public int State;
    }
}