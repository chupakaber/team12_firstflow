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
                    if (Time.time > assistant.LastBehaviorTime + 0.2f)
                    {
                        assistant.LastBehaviorTime = Time.time;

                        if (assistant.TargetBuilding != null)
                        {
                            if (assistant.ResourceBuilding == null)
                            {
                                foreach (var building in Buildings)
                                {
                                    if (building.ProduceItemType == assistant.TargetBuilding.ConsumeItemType)
                                    {
                                        assistant.ResourceBuilding = building;
                                    }
                                }
                            }

                            var resourceAmount = character.Items.GetAmount(assistant.ResourceBuilding.ProduceItemType);
                            var load = (float) resourceAmount / character.ItemLimit;
                            var productionDistance = (assistant.TargetBuilding.UnloadingArea.transform.position - character.transform.position).magnitude;
                            var resourceDistance = (assistant.ResourceBuilding.PickingUpArea.transform.position - character.transform.position).magnitude;
                            var isResourceNeeded = productionDistance / (productionDistance + resourceDistance) > load;
                            var target = isResourceNeeded ? assistant.ResourceBuilding.PickingUpArea : assistant.TargetBuilding.UnloadingArea;
                            var targetPosition = target.transform.position;
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
                        }
                    }
                }
            }
        }
    }
}