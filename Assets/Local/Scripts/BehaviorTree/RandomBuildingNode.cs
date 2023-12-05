using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class RandomBuildingNode : BehaviorCompositeNode, IOutputBuilding
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.35f, 0.5f, 0.2f, 1f); } }

        public int MinimumLevel = 0;
        public List<ItemType> ProduceItemTypes = new List<ItemType>();
        public List<ItemType> ConsumeItemTypes = new List<ItemType>();
        public bool ProductionAreaNeeded;
        public bool PickingUpAreaNeeded;
        public bool UnloadingAreaNeeded;

        private Building _output;

        public RandomBuildingNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(Building);
        }

        public Building GetOutputBuilding()
        {
            return _output;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            var characterState = (SmartCharacterState) internalState;
            var activeBuildingCount = 0;
            foreach (var building in characterState.Buildings)
            {
                if (CheckConstraints(building))
                {
                    activeBuildingCount++;
                }
            }
            var randomIndex = Random.Range(0, activeBuildingCount);
            var i = 0;
            foreach (var building in characterState.Buildings)
            {
                if (CheckConstraints(building))
                {
                    if (i >= randomIndex)
                    {
                        _output = building;
                        break;
                    }
                    i++;
                }
            }
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var childInputIndex = InputTargetIndex[i];
                child.Run(this, childInputIndex, internalState, currentEvent);
            }

            return State.SUCCESS;
        }

        private bool CheckConstraints(Building building)
        {
            if (building.Level >= MinimumLevel 
                && (!ProductionAreaNeeded || building.ProductionArea != null)
                && (!PickingUpAreaNeeded || building.PickingUpArea != null)
                && (!UnloadingAreaNeeded || building.UnloadingArea != null)
                )
            {
                var inProduceItemConstraints = false;
                if (ProduceItemTypes.Count < 1)
                {
                    inProduceItemConstraints = true;
                }
                else
                {
                    foreach (var itemType in ProduceItemTypes)
                    {
                        if (building.ProduceItemType == itemType)
                        {
                            inProduceItemConstraints = true;
                            break;
                        }
                    }
                }
                var inConsumeItemConstraints = false;
                if (ConsumeItemTypes.Count < 1)
                {
                    inConsumeItemConstraints = true;
                }
                else
                {
                    foreach (var itemType in ConsumeItemTypes)
                    {
                        if (building.ConsumeItemType == itemType)
                        {
                            inConsumeItemConstraints = true;
                        }
                    }
                }
                if (inProduceItemConstraints && inConsumeItemConstraints)
                {
                    return true;
                }
            }
            return false;
        }
    }
}