using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class Assistant: Character
    {
        [Header("Assistant Config")]
        public NavMeshAgent NavMeshAgent;
        public float LastBehaviorTime;

        [Header("Assistant Runtime")]
        public Building TargetBuilding;
        public Building ResourceBuilding;
    }
}