namespace Scripts.BehaviorTree
{
    public class ShowEmojiNode : BehaviorCompositeNode
    {
        public int IconIndex;

        public ShowEmojiNode()
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
            var characterState = (SmartCharacterState) internalState;
            characterState.EventBus.CallEvent(new ShowEmojiEvent() { Character = characterState.Self, SpriteIndex = IconIndex });

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