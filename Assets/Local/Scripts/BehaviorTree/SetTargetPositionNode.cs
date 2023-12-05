using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class SetTargetPositionNode : BehaviorCompositeNode
    {
        public override string Input2Name { get { return "Only Offset"; } }

        public float FollowingOffset = 1.7f;
        public bool SaveLastTargetPosition = false;

        public SetTargetPositionNode()
        {
            Input1Type = typeof(Vector3);
            Input2Type = typeof(bool);
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
            if (SaveLastTargetPosition && inputIndex == 1)
            {
                var characterState = (SmartCharacterState) internalState;
                if (characterState.Self is SmartCharacter)
                {
                    characterState.Self.FollowingOffset = FollowingOffset;
                }
            }
            else if (!SaveLastTargetPosition && inputIndex == 0 && parent is IOutputVector3)
            {
                var parentNode = (IOutputVector3) parent;
                var characterState = (SmartCharacterState) internalState;
                if (characterState.Self is SmartCharacter)
                {
                    characterState.Self.TargetPosition = parentNode.GetOutputVector3();
                    characterState.Self.FollowingOffset = FollowingOffset;
                }
            }
            else
            {
                return State.SUCCESS;
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