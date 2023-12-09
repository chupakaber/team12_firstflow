using Scripts.Enums;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class MakeEventNode : BehaviorCompositeNode
    {
        public enum EventTypeEnum {
            ADD_ITEM_TO_CHARACTER = 0,
            REMOVE_ITEM_FROM_CHARACTER = 1,
            REMOVE_ALL_ITEMS_FROM_CHARACTER = 2,
            SET_ARROW_POINTER = 3,
            PLAY_SOUND = 4,
            START_CUTSCENE = 5,
        }

        public EventTypeEnum EventType;

        private float _value1;
        private Vector3 _value2;
        private SmartCharacter _value3;
        private Building _value4;
        private float _value5;
        private bool _hasValue1;
        private bool _hasValue2;
        private bool _hasValue3;
        private bool _hasValue4;
        private bool _hasValue5;

        public MakeEventNode()
        {
            Input1Type = typeof(float);
            Input2Type = typeof(Vector3);
            Input3Type = typeof(SmartCharacter);
            Input4Type = typeof(Building);
            Input5Type = typeof(float);
            Output1Type = typeof(bool);
        }

        public override void Clear()
        {
            base.Clear();
            _hasValue1 = false;
            _hasValue2 = false;
            _hasValue3 = false;
            _hasValue4 = false;
            _hasValue5 = false;
            _value3 = null;
            _value4 = null;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            Clear();
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
            else if (inputIndex == 4 && parent is IOutputFloat)
            {
                var parentNode = (IOutputFloat) parent;
                _value5 = parentNode.GetOutputFloat();
                _hasValue5 = true;
            }
            else
            {
                return State.SUCCESS;
            }

            var success = false;

            switch (EventType)
            {

                case EventTypeEnum.ADD_ITEM_TO_CHARACTER:
                    if (_hasValue1 && _hasValue3 && _hasValue5)
                    {
                        success = true;
                        internalState.EventBus.CallEvent(new AddItemEvent() { Unit = _value3, Count = (int) Mathf.Round(_value5), ItemType = (ItemType) _value1});
                    }
                    break;
                case EventTypeEnum.REMOVE_ALL_ITEMS_FROM_CHARACTER:
                    if (_hasValue3)
                    {
                        success = true;
                        foreach (var item in _value3.Items)
                        {
                            internalState.EventBus.CallEvent(new RemoveItemEvent() { Unit = _value3, Count = item.Amount, ItemType = item.Type});
                        }
                    }
                    break;
                case EventTypeEnum.SET_ARROW_POINTER:
                    if (_hasValue4)
                    {
                        success = true;
                        internalState.EventBus.CallEvent(new SetArrowPointerEvent() { TargetGameObject = _value4.gameObject });
                    }
                    else if (_hasValue2)
                    {
                        if (_value2.magnitude <= float.Epsilon)
                        {
                            internalState.EventBus.CallEvent(new SetArrowPointerEvent() { TargetGameObject = null });
                        }
                    }
                    break;
                case EventTypeEnum.PLAY_SOUND:
                    if (_hasValue1 && (_hasValue3 || _hasValue4))
                    {
                        success = true;
                        internalState.EventBus.CallEvent(new PlaySoundEvent() { SoundId = (int) _value1, IsMusic = false, AttachedTo = ((Unit) (_hasValue3 ? _value3 : _value4)).transform });
                    }
                    break;
                case EventTypeEnum.START_CUTSCENE:
                    if (_hasValue1)
                    {
                        internalState.EventBus.CallEvent(new StartCutSceneEvent() { CutSceneIndex = (int) _value1 });
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
            else
            {
                return State.RUNNING;
            }

            return State.SUCCESS;
        }
    }
}