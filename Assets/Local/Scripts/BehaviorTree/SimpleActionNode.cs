using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class SimpleActionNode : BehaviorActionNode
    {
        public SimpleActionNode()
        {
            Input1Type = typeof(SmartCharacter);
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (parent is IOutputVector3)
            {
                var parentNode = (IOutputCharacter) parent;
                var character = parentNode.GetOutputCharacter();
                var characterState = (SmartCharacterState) internalState;
                if (characterState.Self is SmartCharacter)
                {
                    // characterState.Self.TargetPosition = parentNode.GetOutputVector3();
                }
                Debug.Log($"{Name} | Character.Name: {character.name}");
            }
            return State.SUCCESS;
        }
    }
}