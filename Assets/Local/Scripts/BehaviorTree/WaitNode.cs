using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class WaitNode : BehaviorCompositeNode
    {
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
            var state = (SmartCharacterState) internalState;
            
            if (Time.time > state.Timers[TimerIndex])
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