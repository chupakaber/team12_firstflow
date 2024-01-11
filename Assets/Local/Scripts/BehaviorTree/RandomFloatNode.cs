using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class RandomFloatNode : BehaviorDecoratorNode, IOutputFloat
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.5f, 0.2f, 0.2f, 1f); } }

        public float Min = 0f;
        public float Max = 1f;

        private float _output;

        public RandomFloatNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(float);
        }

        public float GetOutputFloat(int index = 0)
        {
            return _output;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _output = Random.Range(Min, Max);
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