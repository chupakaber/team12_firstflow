using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class FloatValueNode : BehaviorDecoratorNode, IOutputFloat
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.3f, 0.3f, 0.3f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Math"; } }

        public float Value;

        public FloatValueNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(float);
        }

        public float GetOutputFloat(int index = 0)
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