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
                    if (Time.time > apprentice.LastBehaviorTime + 0.2f)
                    {
                        apprentice.LastBehaviorTime = Time.time;

                        if (apprentice.TargetBuilding != null)
                        {
                            var target = apprentice.TargetBuilding.ProductionArea;
                            var targetPosition = target.transform.position;
                            if (apprentice.NavMeshAgent.CalculatePath(targetPosition, _path))
                            {
                                if (_path.GetCornersNonAlloc(_pathCorners) > 1)
                                {
                                    targetPosition = _pathCorners[1];
                                }
                                var delta = targetPosition - character.transform.position;
                                delta.y = 0f;
                                character.WorldDirection = delta.normalized;
                                
                                if (delta.magnitude < 0.5f)
                                {
                                    character.WorldDirection = Vector3.zero;
                                }
                            }
                            else
                            {
                                character.WorldDirection = Vector3.zero;
                            }
                        }
                    }
                }
            }
        }
    }
}