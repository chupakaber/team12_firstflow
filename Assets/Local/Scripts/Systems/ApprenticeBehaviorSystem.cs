using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class ApprenticeBehaviorSystem: ISystem
    {
        public List<Character> Characters;
        public List<Building> Buildings;

        public NavMeshPath _path;
        public Vector3[] _pathCorners = new Vector3[128];

        public void EventCatch(StartEvent newEvent)
        {
            _path = new NavMeshPath();
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.APPRENTICE)
                {
                    var apprentice = (Apprentice) character;
                    if (Time.time > apprentice.LastApprenticeBehaviorTime + 1f)
                    {
                        apprentice.LastApprenticeBehaviorTime = Time.time;

                        if (apprentice.TargetBuilding != null && apprentice.TargetBuilding.ProductionArea != null)
                        {
                            var target = apprentice.TargetBuilding.ProductionArea;
                            apprentice.TargetPosition = target.transform.position;
                            apprentice.FollowingOffset = 0.3f;
                        }
                    }
                }
            }
        }
    }
}