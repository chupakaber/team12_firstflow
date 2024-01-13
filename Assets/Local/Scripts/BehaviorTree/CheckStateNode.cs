using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class CheckStateNode : BehaviorCompositeNode
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.5f, 0.4f, 0.2f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "State"; } }

        public enum CheckStateOperator
        {
            EQUAL = 0,
            NOT_EQUAL = 1,
            GREATER = 2,
            LESS = 3,
            RANGE = 4
        }

        public CheckStateOperator CompareOperator = CheckStateOperator.EQUAL;
        [Header("Constraint for EQUAL / NOT_EQUAL / GREATER / LESS.")]
        public int Value = 0;
        [Header("Constraints for RANGE. Edge values included.")]
        public int Min = 0;
        public int Max = 0;
        [Header("Index of state holder.")]
        public int StateID = 0;

        public CheckStateNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(bool);
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            var success = false;

            switch(CompareOperator)
            {
                case CheckStateOperator.EQUAL:
                    success = internalState.GetState(StateID) == Value;
                    break;
                case CheckStateOperator.NOT_EQUAL:
                    success = internalState.GetState(StateID) != Value;
                    break;
                case CheckStateOperator.GREATER:
                    success = internalState.GetState(StateID) > Value;
                    break;
                case CheckStateOperator.LESS:
                    success = internalState.GetState(StateID) < Value;
                    break;
                case CheckStateOperator.RANGE:
                    success = internalState.GetState(StateID) >= Min && internalState.GetState(StateID) < Max;
                    break;
            }

            if (success)
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    var child = Children[i];
                    var childInputIndex = InputTargetIndex[i];
                    child.Run(this, childInputIndex, internalState, currentEvent);
                }
            }
            
            return State.SUCCESS;
        }
    }
}