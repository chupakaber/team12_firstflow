using Scripts.Enums;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class MakeEventNode : BehaviorCompositeNode
    {
        public enum EventTypeEnum {
            ADD_ITEM_TO_CHARACTER = 0,
            REMOVE_ITEM_FROM_CHARACTER = 1,
            REMOVE_ALL_ITEMS_FROM_CHARACTER = 2
        }

        public EventTypeEnum EventType;

        private float _value1;
        private Vector3 _value2;
        private SmartCharacter _value3;
        private Building _value4;
        private bool _hasValue1;
        private bool _hasValue2;
        private bool _hasValue3;
        private bool _hasValue4;

        public MakeEventNode()
        {
            Input1Type = typeof(float);
            Input2Type = typeof(Vector3);
            Input3Type = typeof(SmartCharacter);
            Input4Type = typeof(Building);
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
            if (inputIndex == 0 && parent is IOutputFloat)
            {
                var parentNode = (IOutputFloat) parent;
                _value1 = parentNode.GetOutputFloat();
                _hasValue1 = true;
            }
            else if (inputIndex == 1 && parent is IOutputVector3)
            {
                var parentNode = (IOutputVector3) parent;
                _value2 = parentNode.GetOutputVector3();
                _hasValue2 = true;
            }
            else if (inputIndex == 2 && parent is IOutputCharacter)
            {
                var parentNode = (IOutputCharacter) parent;
                _value3 = parentNode.GetOutputCharacter();
                _hasValue3 = true;
            }
            else if (inputIndex == 3 && parent is IOutputBuilding)
            {
                var parentNode = (IOutputBuilding) parent;
                _value4 = parentNode.GetOutputBuilding();
                _hasValue4 = true;
            }
            else
            {
                return State.SUCCESS;
            }

            var success = false;

            var characterState = (SmartCharacterState) internalState;
            switch (EventType)
            {

                case EventTypeEnum.ADD_ITEM_TO_CHARACTER:
                    if (_hasValue1 && _hasValue3)
                    {
                        characterState.EventBus.CallEvent(new AddItemEvent() { Unit = _value3, Count = 1, ItemType = (ItemType) _value1});
                    }
                    break;
                case EventTypeEnum.REMOVE_ALL_ITEMS_FROM_CHARACTER:
                    if (_hasValue3)
                    {
                        foreach (var item in _value3.Items)
                        {
                            characterState.EventBus.CallEvent(new RemoveItemEvent() { Unit = _value3, Count = item.Amount, ItemType = item.Type});
                        }
                    }
                    break;
            }

            if (success)
            {
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