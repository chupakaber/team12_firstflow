using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class SetCharacterValueNode : BehaviorCompositeNode
    {
        public enum FieldNameEnum {
            FOLLOWING_OFFSET = 0,
            TARGET_POSITION = 1,
            TARGET_CHARACTER = 2,
            STOP_WORKER = 3
        }

        public FieldNameEnum FieldName;

        private bool _hasValue1;
        private bool _hasValue2;
        private bool _hasValue3;

        public SetCharacterValueNode()
        {
            Input1Type = typeof(Vector3);
            Input2Type = typeof(float);
            Input3Type = typeof(SmartCharacter);
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
            if (inputIndex == 1 && parent is IOutputFloat)
            {
                var parentNode = (IOutputFloat) parent;
                var characterState = (SmartCharacterState) internalState;
                if (characterState.Self is SmartCharacter)
                {
                    var value = parentNode.GetOutputFloat();
                    switch (FieldName)
                    {
                        case FieldNameEnum.FOLLOWING_OFFSET:
                            characterState.Self.FollowingOffset = value;
                            break;
                    }
                }
            }
            else if (inputIndex == 0 && parent is IOutputVector3)
            {
                var parentNode = (IOutputVector3) parent;
                var characterState = (SmartCharacterState) internalState;
                if (characterState.Self is SmartCharacter)
                {
                    var value = parentNode.GetOutputVector3();
                    switch (FieldName)
                    {
                        case FieldNameEnum.TARGET_POSITION:
                            characterState.Self.TargetPosition = value;
                            break;
                    }
                }
            }
            else if (inputIndex == 2 && parent is IOutputCharacter)
            {
                var parentNode = (IOutputCharacter) parent;
                var characterState = (SmartCharacterState) internalState;
                if (characterState.Self is SmartCharacter)
                {
                    var value = parentNode.GetOutputCharacter();
                    switch (FieldName)
                    {
                        case FieldNameEnum.TARGET_CHARACTER:
                            characterState.Self.TargetCharacter = value;
                            break;
                        case FieldNameEnum.STOP_WORKER:
                            if (value is Worker)
                            {
                                var worker = (Worker) value;
                                worker.LastBehaviorTime = Time.time + 5f;
                                worker.TargetPosition = Vector3.zero;
                                worker.WorldDirection = Vector3.zero;
                                if (worker is Assistant)
                                {
                                    var assistant = (Assistant) worker;
                                    assistant.LastAssistantBehaviorTime = Time.time + 5f;
                                }
                            }
                            break;
                    }
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