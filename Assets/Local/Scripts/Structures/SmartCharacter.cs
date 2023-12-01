using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class SmartCharacter: Character
    {
        [Header("SmartCharacter Config")]
        public NavMeshAgent NavMeshAgent;

        [Header("SmartCharacter Runtime")]
        public float LastBehaviorTime;
        public CustomerRoute Route;
        public int CurrentRouteWaypointIndex;
    }
}