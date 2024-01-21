using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class SelfNode : BehaviorCompositeNode, IOutputCharacter
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.35f, 0.5f, 0.2f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Object"; } }

        [HideInInspector]
        public SmartCharacter Output;

        public SelfNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(SmartCharacter);
        }

        public SmartCharacter GetOutputCharacter()
        {
            return Output;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            var characterState = (SmartCharacterState) internalState;
            Output = characterState.Self;
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