using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class SequenceNode : BehaviorCompositeNode
    {
        [HideInInspector]
        public override string Section { get { return "Logic"; } }

        public SequenceNode()
        {
            var type = typeof(bool);
            Input1Type = type;
            Output1Type = type;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
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
    }
}