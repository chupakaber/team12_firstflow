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
        private List<int> _dynamicIndexList;

        public RandomBuildingNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(Building);
        }

        public Building GetOutputBuilding()
        {
            return _output;
        }

        public override void Clear()
        {
            base.Clear();
            _output = null;
            if (_dynamicIndexList != null)
            {
                _dynamicIndexList.Clear();
            }
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (ForEach)
            {
                if (_dynamicIndexList == null)
                {
                    _dynamicIndexList = new List<int>();
                }
                _dynamicIndexList.Clear();
            }

            var activeBuildingCount = 0;
            for (var i = 0; i < internalState.Buildings.Count; i++)
            {
                var building = internalState.Buildings[i];
                if (CheckConstraints(building))
                {
                    if (ForEach)
                    {
                        _dynamicIndexList.Add(i);
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
            
            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var childInputIndex = InputTargetIndex[i];
                if (ForEach)
                {
                    foreach (var buildingIndex in _dynamicIndexList)
                    {
                        _output = internalState.Buildings[buildingIndex];
                        child.Run(this, childInputIndex, internalState, currentEvent);
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