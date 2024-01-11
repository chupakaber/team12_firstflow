using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class Vector3ValueNode : BehaviorDecoratorNode, IOutputVector3
    {
        public Vector3 Value;

        public Vector3ValueNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(Vector3);
        }

        public Vector3 GetOutputVector3()
        {
            return Value;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (Child != null)
            {
                Child.Run(this, InputTargetIndex, internalState, currentEvent);
            }

            return State.SUCCESS;
        }
    }
}