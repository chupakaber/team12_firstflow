using UnityEngine;
using UnityEngine.AI;
using Scripts.BehaviorTree;

namespace Scripts
{
    public class SmartCharacter: Character
    {
        [Header("SmartCharacter Config")]
        public NavMeshAgent NavMeshAgent;
        public BehaviorTreeRunner BehaviorTree;

        [Header("SmartCharacter Runtime")]
        public float LastBehaviorTime;
        public CustomerRoute Route;
        public int CurrentRouteWaypointIndex;
        public Building TargetBuilding;
        public Character TargetCharacter;
        public Vector3 TargetPosition;
        public float FollowingOffset;
        public Vector3 SpawnPoint;

        [Header("SmartCharacter Debug")]
        public SmartCharacterState InternalState;
    }
}