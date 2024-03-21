using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class ShowUIEffectNode : BehaviorCompositeNode
    {
        [HideInInspector]
        public override string Section { get { return "Action"; } }
        
        [Header("1 = First Honor Animation")]
        public int EffectID;

        public ShowUIEffectNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(bool);
        }

        public override void Clear()
        {
            base.Clear();
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            internalState.EventBus.CallEvent(new ShowUIEffectEvent() { EffectID = EffectID });

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