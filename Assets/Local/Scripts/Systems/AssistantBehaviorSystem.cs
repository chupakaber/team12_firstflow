using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class AssistantBehaviorSystem: ISystem
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
                if (character.CharacterType == CharacterType.ASSISTANT)
                {
                    var assistant = (Assistant) character;
                    if (Time.time > assistant.LastAssistantBehaviorTime + 1f)
                    {
                        assistant.LastAssistantBehaviorTime = Time.time;

                        if (
                            assistant.TargetBuilding != null && 
                            assistant.TargetBuilding.UnloadingArea != null && 
                            assistant.ResourceBuilding != null && 
                            assistant.ResourceBuilding.PickingUpArea != null)
                        {
                            var totalAmount = character.Items.GetAmount();
                            var resourceAmount = character.Items.GetAmount(assistant.TargetBuilding.ConsumeItemType);
                            var load = (float) resourceAmount / Mathf.Max(1, character.ItemLimit - (totalAmount - resourceAmount));
                            var productionDistance = (assistant.TargetBuilding.UnloadingArea.transform.position - character.transform.position).magnitude;
                            var resourceDistance = (assistant.ResourceBuilding.PickingUpArea.transform.position - character.transform.position).magnitude;
                            var isResourceNeeded = productionDistance / (productionDistance + resourceDistance) > load;
                            var target = isResourceNeeded ? assistant.ResourceBuilding.PickingUpArea : assistant.TargetBuilding.UnloadingArea;
                            //var targetPosition = target.transform.position;

                            assistant.TargetPosition = target.transform.position;
                            assistant.FollowingOffset = 0.7f;
                            /*
                            if (assistant.NavMeshAgent.CalculatePath(targetPosition, _path))
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
                            */
                        }
                    }
                }
            }
        }
    }
}