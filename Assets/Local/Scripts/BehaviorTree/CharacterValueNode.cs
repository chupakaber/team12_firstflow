using Scripts.Enums;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class CharacterValueNode : BehaviorCompositeNode, IOutputFloat, IOutputVector3
    {
        public enum FieldNameEnum
        {
            POSITION = 0,
            ITEM_LIMIT = 1,
            ITEMS_AMOUNT = 2,
            TARGET_POSITION = 3,
            TARGET_OFFSET = 4,
            SPAWN_POINT = 5,
            TYPE = 6
        }

        public FieldNameEnum FieldName;
        public bool IsSecondInputValueNeeded = false;

        private SmartCharacter _value1;
        private float _value2;
        private bool _hasValue1;
        private bool _hasValue2;

        public CharacterValueNode()
        {
            Input1Type = typeof(SmartCharacter);
            Input2Type = typeof(float);
            Output1Type = typeof(float);
            Output2Type = typeof(Vector3);
        }

        public float GetOutputFloat(int index = 0)
        {
            switch (FieldName)
            {
                case FieldNameEnum.ITEMS_AMOUNT:
                    if (IsSecondInputValueNeeded && _hasValue2)
                    {
                        return _value1.Items.GetAmount((ItemType) _value2);
                    }
                    return _value1.Items.GetAmount();
                case FieldNameEnum.ITEM_LIMIT:
                    return _value1.ItemLimit;
                case FieldNameEnum.TARGET_OFFSET:
                    return _value1.FollowingOffset;
                case FieldNameEnum.TYPE:
                    return (float) _value1.CharacterType;
            }

            return 0f;
        }

        public Vector3 GetOutputVector3()
        {
            switch (FieldName)
            {
                case FieldNameEnum.POSITION:
                    return _value1.transform.position;
                case FieldNameEnum.TARGET_POSITION:
                    return _value1.TargetPosition;
                case FieldNameEnum.SPAWN_POINT:
                    return _value1.SpawnPoint;
            }

            return Vector3.zero;
        }

        public override void Clear()
        {
            base.Clear();
            _value1 = null;
            _hasValue1 = false;
            _hasValue2 = false;
        }

        private bool IsIndexRequired()
        {
            return IsSecondInputValueNeeded;
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
                _value1 = parentNode.GetOutputCharacter();
                _hasValue1 = true;
            }

            if (inputIndex == 1 && parent is IOutputFloat)
            {
                var parentNode = (IOutputFloat) parent;
                _value2 = parentNode.GetOutputFloat();
                _hasValue2 = true;
            }
            
            if (_value1 == null || (!_hasValue2 && IsIndexRequired()))
            {
                return State.RUNNING;
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