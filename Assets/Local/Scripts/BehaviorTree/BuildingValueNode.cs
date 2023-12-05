using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class BuildingValueNode : BehaviorCompositeNode, IOutputFloat, IOutputVector3
    {
        public enum FieldNameEnum
        {
            POSITION = 0,
            PICKING_UP_AREA_POSITION = 1,
            ITEMS_AMOUNT = 2,
        }

        public FieldNameEnum FieldName;

        [HideInInspector]
        public int Index;

        private Building _building;
        private bool _hasIndexValue;

        public BuildingValueNode()
        {
            Input1Type = typeof(Building);
            Input2Type = typeof(int);
            Output1Type = typeof(float);
            Output2Type = typeof(Vector3);
        }

        public float GetOutputFloat()
        {
            switch (FieldName)
            {
                case FieldNameEnum.ITEMS_AMOUNT:
                    return _building.Items.GetAmount();
            }

            return 0f;
        }

        public Vector3 GetOutputVector3()
        {
            switch (FieldName)
            {
                case FieldNameEnum.POSITION:
                    return _building.transform.position;
                case FieldNameEnum.PICKING_UP_AREA_POSITION:
                    return _building.PickingUpArea.transform.position;
            }

            return Vector3.zero;
        }

        public override void Clear()
        {
            base.Clear();
            _building = null;
            _hasIndexValue = false;
        }

        private bool IsIndexRequired()
        {
            // if (FieldName == FieldNameEnum.)
            // return true;
            return false;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {

        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (inputIndex == 0 && parent is IOutputBuilding)
            {
                var parentNode = (IOutputBuilding) parent;
                _building = parentNode.GetOutputBuilding();
            }

            if (inputIndex == 1 && parent is IOutputFloat)
            {
                var parentNode = (IOutputFloat) parent;
                Index = (int) parentNode.GetOutputFloat();
                _hasIndexValue = true;
            }
            
            if (_building == null || (!_hasIndexValue && IsIndexRequired()))
            {
                return State.RUNNING;
            }

            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var childInputIndex = InputTargetIndex[i];
                child.Run(this, childInputIndex, internalState, currentEvent);
            }

            return State.SUCCESS;
        }
    }
}