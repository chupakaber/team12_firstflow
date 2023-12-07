using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class SetStateNode : BehaviorCompositeNode
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.5f, 0.4f, 0.2f, 1f); } }

        public enum SetStateMethod
        {
            OVERWRITE = 0,
            INCREMENT = 1
        }

        public SetStateMethod Method = SetStateMethod.OVERWRITE;
        public int Value = 0;
        public int StateID = 0;

        public SetStateNode()
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
            if (Method == SetStateMethod.OVERWRITE)
            {
                internalState.SetState(StateID, Value);
            }
            else if (Method == SetStateMethod.INCREMENT)
            {
                internalState.SetState(StateID, internalState.GetState(StateID) + Value);
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