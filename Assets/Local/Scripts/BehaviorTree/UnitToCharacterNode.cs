using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class UnitToCharacterNode : BehaviorCompositeNode, IOutputCharacter
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.35f, 0.5f, 0.2f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Convert"; } }

        private SmartCharacter _output;

        public UnitToCharacterNode()
        {
            Input1Type = typeof(Unit);
            Output1Type = typeof(SmartCharacter);
        }

        public SmartCharacter GetOutputCharacter()
        {
            return _output;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _output = null;
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _output = null;
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (parent is IOutputUnit)
            {
                var unit = ((IOutputUnit) parent).GetOutputUnit();
                if (unit is SmartCharacter)
                {
                    _output = (SmartCharacter) unit;
                }
            }
            
            if (_output == null)
            {
                return State.FAILURE;
            }

            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var childInputIndex = InputTargetIndex[i];
                child.Run(this, childInputIndex, internalState, currentEvent);
            }

            return State.FAILURE;
        }

        public override void Clear()
        {
            base.Clear();
            _output = null;
        }
    }
}