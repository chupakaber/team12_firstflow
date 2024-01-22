using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class GetTargetCharacterNode : BehaviorCompositeNode, IOutputCharacter
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.35f, 0.5f, 0.2f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Get"; } }

        private SmartCharacter _output;

        public GetTargetCharacterNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(SmartCharacter);
        }

        public SmartCharacter GetOutputCharacter()
        {
            return _output;
        }

        public override void Clear()
        {
            base.Clear();
            _output = null;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            var characterState = (SmartCharacterState) internalState;
            if (characterState.Self.TargetCharacter is SmartCharacter)
            {
                _output = (SmartCharacter) characterState.Self.TargetCharacter;
                
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