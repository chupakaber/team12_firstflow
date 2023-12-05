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
        }

        public FieldNameEnum FieldName;

        [HideInInspector]
        public SmartCharacter Character;
        [HideInInspector]
        public int Index;

        private bool _hasIndexValue;

        public CharacterValueNode()
        {
            Input1Type = typeof(SmartCharacter);
            Input2Type = typeof(int);
            Output1Type = typeof(float);
            Output2Type = typeof(Vector3);
        }

        public float GetOutputFloat()
        {
            switch (FieldName)
            {
                case FieldNameEnum.ITEMS_AMOUNT:
                    return Character.Items.GetAmount();
                case FieldNameEnum.ITEM_LIMIT:
                    return Character.ItemLimit;
                case FieldNameEnum.TARGET_OFFSET:
                    return Character.FollowingOffset;
            }

            return 0f;
        }

        public Vector3 GetOutputVector3()
        {
            switch (FieldName)
            {
                case FieldNameEnum.POSITION:
                    return Character.transform.position;
                case FieldNameEnum.TARGET_POSITION:
                    return Character.TargetPosition;
                case FieldNameEnum.SPAWN_POINT:
                    return Character.SpawnPoint;
            }

            return Vector3.zero;
        }

        public override void Clear()
        {
            base.Clear();
            Character = null;
            _hasIndexValue = false;
        }

        private bool IsIndexRequired()
        {
            // if (FieldName == FieldNameEnum.)
            // return true;
            return false;
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
                Character = parentNode.GetOutputCharacter();
            }

            if (inputIndex == 1 && parent is IOutputFloat)
            {
                var parentNode = (IOutputFloat) parent;
                Index = (int) parentNode.GetOutputFloat();
                _hasIndexValue = true;
            }
            
            if (Character == null || (!_hasIndexValue && IsIndexRequired()))
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