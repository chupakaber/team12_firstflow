namespace Scripts.BehaviorTree
{
    public class ShowMessageNode : BehaviorCompositeNode
    {
        public string Message;

        private SmartCharacter _value2;

        private bool _hasValue1;
        private bool _hasValue2;

        public ShowMessageNode()
        {
            Input1Type = typeof(bool);
            Input2Type = typeof(SmartCharacter);
            Output1Type = typeof(bool);
        }

        public override void Clear()
        {
            base.Clear();
            _hasValue1 = false;
            _hasValue2 = false;
            _value2 = null;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (inputIndex == 0)
            {
                _hasValue1 = true;
            }
            else if (inputIndex == 1 && parent is IOutputCharacter)
            {
                var parentNode = (IOutputCharacter) parent;
                _value2 = parentNode.GetOutputCharacter();
                _hasValue2 = true;
            }
            else
            {
                return State.SUCCESS;
            }

            if (_hasValue1 && _hasValue2)
            {
                internalState.EventBus.CallEvent(new ShowMessageEvent() { Character = _value2, Message = Message });

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