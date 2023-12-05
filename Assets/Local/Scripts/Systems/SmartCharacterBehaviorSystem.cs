using System.Collections.Generic;
using Scripts.BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class SmartCharacterBehaviorSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public NavMeshPath _path;
        public Vector3[] _pathCorners = new Vector3[128];

        private BehaviorTree.BehaviorTree _originalBehaviorTree;

        public void EventCatch(StartEvent newEvent)
        {
            _path = new NavMeshPath();
            _originalBehaviorTree = Resources.Load<BehaviorTree.BehaviorTree>("BehaviorTrees/BehaviorTree");
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var character in Characters)
            {
                if (character is SmartCharacter)
                {
                    var smartCharacter = (SmartCharacter) character;
                    
                    if (smartCharacter.BehaviorTree != null)
                    {
                        if (smartCharacter.BehaviorTree.Tree == null)
                        {
                            smartCharacter.BehaviorTree.Tree = ScriptableObject.Instantiate(_originalBehaviorTree);
                            smartCharacter.BehaviorTree.InternalState = new SmartCharacterState()
                            {
                                EventBus = EventBus,
                                Self = smartCharacter,
                                Characters = Characters,
                                Buildings = Buildings
                            };

                            smartCharacter.InternalState = (SmartCharacterState) smartCharacter.BehaviorTree.InternalState;
                        }

                        smartCharacter.BehaviorTree.EventCatch(newEvent);






                    
                        if (smartCharacter.TargetPosition.sqrMagnitude > float.Epsilon * 2f)
                        {
                            var workerPosition = smartCharacter.transform.position;

                            var toTargetDelta = smartCharacter.TargetPosition - workerPosition;
                            toTargetDelta.y = 0f;
                            var toTargetDistance = toTargetDelta.magnitude;

                            if (toTargetDistance < smartCharacter.FollowingOffset)
                            {
                                character.WorldDirection = Vector3.zero;
                            }
                            else
                            {
                                var pathPosition = smartCharacter.TargetPosition;

                                smartCharacter.NavMeshAgent.enabled = true;
                                if (smartCharacter.NavMeshAgent.CalculatePath(smartCharacter.TargetPosition, _path))
                                {
                                    var cornersCount = _path.GetCornersNonAlloc(_pathCorners);
                                    if (cornersCount > 1)
                                    {
                                        pathPosition = _pathCorners[1];
                                    }
                                    else if (cornersCount > 0)
                                    {
                                        pathPosition = _pathCorners[0];
                                    }
                                }
                                smartCharacter.NavMeshAgent.enabled = false;

                                var pathDelta = pathPosition - workerPosition;
                                // pathDelta.y = 0f;
                                character.WorldDirection = pathDelta.normalized * Mathf.Min(Mathf.Max(pathDelta.magnitude, 0.1f), 1f);

                                if (Physics.Raycast(character.transform.position + Vector3.up * 1.7f, character.WorldDirection, out var hitInfo, 2f))
                                {
                                    if (hitInfo.collider is CapsuleCollider)
                                    {
                                        character.WorldDirection = Quaternion.Euler(0f, 45f, 0f) * character.WorldDirection;
                                    }
                                }
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