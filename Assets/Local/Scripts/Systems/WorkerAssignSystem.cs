using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class WorkerAssignSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public NavMeshPath _path;
        public Vector3[] _pathCorners = new Vector3[128];

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            if (newEvent.Character.CharacterType == CharacterType.PLAYER)
            {
                foreach (var building in Buildings)
                {
                    var trigger = newEvent.Trigger;
                    if (trigger == building.ProductionArea || trigger == building.PickingUpArea || trigger == building.UnloadingArea || trigger == building.UpgradeArea)
                    {
                        if (building.ProduceItemType != ItemType.NONE && building.ProduceItemType != ItemType.ASSISTANT && building.ProduceItemType != ItemType.APPRENTICE)
                        {
                            if ((building.AssignedPickingUpCharacters.Count == 0 && building.PickingUpArea != null) || (building.AssignedProductionCharacters.Count == 0 && building.ProductionArea != null))
                            {
                                foreach (var character in Characters)
                                {
                                    if (character.CharacterType == CharacterType.ASSISTANT && building.AssignedPickingUpCharacters.Count == 0 && building.PickingUpArea != null && building.ProduceItemType != ItemType.GOLD)
                                    {
                                        var assistant = (Assistant) character;
                                        if (assistant.ResourceBuilding == null && assistant.TargetCharacter != null)
                                        {
                                            foreach (var building2 in Buildings)
                                            {
                                                if (building2.Level > 0 && building2.ConsumeItemType == building.ProduceItemType && building2.UnloadingArea != null)
                                                {
                                                    if (assistant.TargetBuilding == null || assistant.TargetBuilding.AssignedUnloadingCharacters.Count > building2.AssignedUnloadingCharacters.Count)
                                                    {
                                                        assistant.TargetBuilding = building2;
                                                    }
                                                }
                                            }

                                            if (assistant.TargetBuilding != null)
                                            {
                                                assistant.ResourceBuilding = building;
                                                building.AssignedPickingUpCharacters.Add(assistant);

                                                assistant.TargetBuilding.AssignedUnloadingCharacters.Add(assistant);

                                                assistant.PickUpItemConstraint = building.ProduceItemType;

                                                while (assistant.Items.TryGetFirstItem(out var type, out var amount))
                                                {
                                                    var removeItemEvent = new RemoveItemEvent() { ItemType = type, Count = amount, Unit = assistant };
                                                    EventBus.CallEvent(removeItemEvent);
                                                }

                                                assistant.TargetCharacter = null;
                                            }
                                        }
                                    }

                                    if (character.CharacterType == CharacterType.APPRENTICE && building.AssignedProductionCharacters.Count == 0 && building.ProductionArea != null)
                                    {
                                        var apprentice = (Worker) character;
                                        if (apprentice.TargetBuilding == null && apprentice.TargetCharacter != null)
                                        {
                                            apprentice.TargetBuilding = building;
                                            building.AssignedProductionCharacters.Add(apprentice);

                                            apprentice.TargetCharacter = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}