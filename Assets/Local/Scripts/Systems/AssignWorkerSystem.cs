using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public class AssignWorkerSystem: ISystem
    {
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
                            var isBuildingAssistantAssigned = false;
                            var isBuildingApprenticeAssigned = false;

                            foreach (var character in Characters)
                            {
                                if (character.CharacterType == CharacterType.ASSISTANT || character.CharacterType == CharacterType.APPRENTICE)
                                {
                                    var worker = (Worker) character;
                                    if (worker.TargetBuilding == building)
                                    {
                                        if (character.CharacterType == CharacterType.ASSISTANT)
                                        {
                                            isBuildingAssistantAssigned = true;
                                        }
                                        if (character.CharacterType == CharacterType.APPRENTICE)
                                        {
                                            isBuildingApprenticeAssigned = true;
                                        }
                                    }
                                }
                            }

                            if (!isBuildingAssistantAssigned && building.UnloadingArea != null)
                            {
                                foreach (var character in Characters)
                                {
                                    if (character.CharacterType == CharacterType.ASSISTANT)
                                    {
                                        var worker = (Worker) character;
                                        worker.TargetBuilding = building;
                                        break;
                                    }
                                }
                            }

                            if (!isBuildingApprenticeAssigned && building.ProductionArea != null)
                            {
                                foreach (var character in Characters)
                                {
                                    if (character.CharacterType == CharacterType.APPRENTICE)
                                    {
                                        var worker = (Worker) character;
                                        worker.TargetBuilding = building;
                                        break;
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