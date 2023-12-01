using UnityEngine;

namespace Scripts
{
    public class Worker: SmartCharacter
    {
        [Header("Worker Runtime")]
        public Building TargetBuilding;
        public Vector3 TargetPosition;
        public float FollowingOffset;
    }
}