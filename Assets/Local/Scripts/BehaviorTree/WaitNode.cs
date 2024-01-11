using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class WaitNode : BehaviorCompositeNode
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.6f, 0.6f, 0.6f, 1f); } }
        
        public int TimerIndex = 0;

        public WaitNode()
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
            if (Time.time > internalState.GetTimer(TimerIndex))
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    var child = Children[i];
                    var childInputIndex = InputTargetIndex[i];
                    child.Run(this, childInputIndex, internalState, currentEvent);
                }

                return State.SUCCESS;
            }

            return State.RUNNING;
        }
    }
}