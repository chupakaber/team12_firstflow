namespace Scripts.BehaviorTree
{
    public class JoinQueueNode : BehaviorCompositeNode
    {
        public bool AtLast= true;

        public JoinQueueNode()
        {
            Input1Type = typeof(SmartCharacter);
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
            if (inputIndex == 0 && parent is IOutputCharacter)
            {
                var parentNode = (IOutputCharacter) parent;
                var characterState = (SmartCharacterState) internalState;
                var character = parentNode.GetOutputCharacter();
                if (character.NextInQueue != null || character.PreviousInQueue != null)
                {
                    if (AtLast)
                    {
                        character.AddLastInQueue(characterState.Self);
                    }
                    else
                    {
                        character.AddFirstInQueue(characterState.Self);
                    }

                    for (var i = 0; i < Children.Count; i++)
                    {
                        var child = Children[i];
                        var childInputIndex = InputTargetIndex[i];
                        child.Run(this, childInputIndex, internalState, currentEvent);
                    }
                }
            }

            return State.SUCCESS;
        }
    }
}