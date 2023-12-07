using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class RandomBuildingNode : BehaviorCompositeNode, IOutputBuilding
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.35f, 0.5f, 0.2f, 1f); } }

        public int ID = -1;
        public int MinimumLevel = 0;
        public int MaximumLevel = 1000;
        public List<ItemType> ProduceItemTypes = new List<ItemType>();
        public List<ItemType> ConsumeItemTypes = new List<ItemType>();
        public bool ProductionAreaNeeded;
        public bool PickingUpAreaNeeded;
        public bool UnloadingAreaNeeded;

        [Header("Cycle")]
        public bool ForEach = false;

        private Building _output;
        private List<Building> _outputForEach = new List<Building>();
        private int _currentIndex;

        public RandomBuildingNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(Building);
        }

        public Building GetOutputBuilding()
        {
            if (ForEach)
            {
                return _outputForEach[_currentIndex];
            }

            return _output;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _outputForEach.Clear();

            var activeBuildingCount = 0;
            foreach (var building in internalState.Buildings)
            {
                if (CheckConstraints(building))
                {
                    if (ForEach)
                    {
                        _outputForEach.Add(building);
                    }
                    else
                    {
                        activeBuildingCount++;
                    }
                }
            }

            if (!ForEach)
            {
                var randomIndex = Random.Range(0, activeBuildingCount);
                var i = 0;
                foreach (var building in internalState.Buildings)
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
                if (ForEach)
                {
                    _currentIndex = 0;
                    foreach (var building in _outputForEach)
                    {
                        child.Run(this, childInputIndex, internalState, currentEvent);
                        _currentIndex++;
                    }
                }
                else
                {
                    child.Run(this, childInputIndex, internalState, currentEvent);
                }
            }

            return State.SUCCESS;
        }

        private bool CheckConstraints(Building building)
        {
            if (ID > -1 && building.ID != ID)
            {
                return false;
            }

            if (building.Level < MinimumLevel || building.Level > MaximumLevel)
            {
                return false;
            }

            if ((!ProductionAreaNeeded || building.ProductionArea != null)
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