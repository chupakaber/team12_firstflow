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
            if (newEvent.Character.CharacterType == CharacterType.PLAYER && newEvent.Character.NextInQueue != null)
            {
                foreach (var building in Buildings)
                {
                    var trigger = newEvent.Trigger;
                    if (trigger == building.ProductionArea || trigger == building.PickingUpArea || trigger == building.UnloadingArea || trigger == building.UpgradeArea)
                    {
                        if (building.ProduceItemType != ItemType.NONE && building.ProduceItemType != ItemType.ASSISTANT && building.ProduceItemType != ItemType.APPRENTICE && building.ProduceItemType != ItemType.BOTTLE_HERO && building.ProduceItemType != ItemType.BOTTLE_WORKER && building.ProduceItemType != ItemType.EMPEROR_SWORD)
                        {
                            if ((building.AssignedPickingUpCharacters.Count == 0 && building.PickingUpArea != null) || (building.AssignedProductionCharacters.Count == 0 && building.ProductionArea != null))
                            {
                                foreach (var character in Characters)
                                {
                                    if (character.CharacterType == CharacterType.ASSISTANT && building.AssignedPickingUpCharacters.Count == 0 && building.PickingUpArea != null && building.ProduceItemType != ItemType.GOLD)
                                    {
                                        var assistant = (Assistant) character;
                                        if (assistant.ResourceBuilding == null && assistant.PreviousInQueue != null)
                                        {
                                            foreach (var building2 in Buildings)
                                            {
                                                if (building2.Level > 0 && building2.ConsumeItemType == building.ProduceItemType && building2.UnloadingArea != null)
                                                {
                                                    if (assistant.TargetBuilding == null || assistant.TargetBuilding.AssignedUnloadingCharacters.Count > building2.AssignedUnloadingCharacters.Count)
                                                    {
                                                        if (building2.AssignedUnloadingCharacters.Count == 0 || building2.ProduceItemType == ItemType.GOLD)
                                                        {
                                                            assistant.TargetBuilding = building2;
                                                        }
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

                                                assistant.LeaveQueue();

                                                UpdateAssignmentHelper();

                                                EventBus.CallEvent(new AssignmentEvent() {
                                                    Character = assistant,
                                                    Building = assistant.ResourceBuilding,
                                                    Area = assistant.TargetBuilding.UnloadingArea.gameObject
                                                });
                                            }
                                        }
                                    }

                                    if (character.CharacterType == CharacterType.APPRENTICE && building.AssignedProductionCharacters.Count == 0 && building.ProductionArea != null)
                                    {
                                        var apprentice = (Worker) character;
                                        if (apprentice.TargetBuilding == null && apprentice.PreviousInQueue != null)
                                        {
                                            apprentice.TargetBuilding = building;
                                            building.AssignedProductionCharacters.Add(apprentice);

                                            apprentice.LeaveQueue();

                                            EventBus.CallEvent(new AssignmentEvent() {
                                                Character = apprentice,
                                                Building = apprentice.TargetBuilding,
                                                Area = apprentice.TargetBuilding.ProductionArea.gameObject
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void EventCatch(StartEvent newEvent)
        {
            UpdateAssignmentHelper();
        }

        public void EventCatch(ConstructionCompleteEvent newEvent)
        {
            UpdateAssignmentHelper();
        }

        public void EventCatch(RemoveItemEvent newEvent)
        {
            UpdateAssignmentHelper();
        }

        private void UpdateAssignmentHelper()
        {
            var hasAssistant = false;
            var hasApprentice = false;
            foreach (var character in Characters)
            {
                var member = character.NextInQueue;
                while (member != null)
                {
                    if (character.NextInQueue.CharacterType == CharacterType.ASSISTANT)
                    {
                        hasAssistant = true;
                    }
                    else if (character.NextInQueue.CharacterType == CharacterType.APPRENTICE)
                    {
                        hasApprentice = true;
                    }

                    member = member.NextInQueue;
                }
            }
            foreach (var building in Buildings)
            {
                if (building.PickingUpAreaHelper != null)
                {
                    building.PickingUpAreaHelper.SetActive(building.AssignedPickingUpCharacters.Count == 0 && hasAssistant);
                }
                if (building.ProductionAreaHelper != null)
                {
                    building.ProductionAreaHelper.SetActive((building.AssignedProductionCharacters.Count == 0 && hasApprentice)
                        || ((hasApprentice || hasAssistant) && building.ConsumeItemType == ItemType.NONE && building.ProduceItemType == ItemType.GOLD));
                }
            }
        }
    }
}