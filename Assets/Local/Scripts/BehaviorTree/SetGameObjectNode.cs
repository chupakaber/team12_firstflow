using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class SetGameObjectValueNode : BehaviorCompositeNode
    {
        public enum FieldNameEnum {
            ACTIVE = 0,
        }

        public FieldNameEnum FieldName;

        private GameObject _inputValue1;
        private float _inputValue2;
        private bool _hasValue1;
        private bool _hasValue2;

        public SetGameObjectValueNode()
        {
            Input1Type = typeof(GameObject);
            Input2Type = typeof(float);
            Output1Type = typeof(bool);
        }

        public override void Clear()
        {
            base.Clear();
            _inputValue1 = null;
            _hasValue1 = false;
            _hasValue2 = false;
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
                _inputValue2 = parentNode.GetOutputFloat();
                _hasValue2 = true;
            }
            else if (inputIndex == 0 && parent is IOutputGameObject)
            {
                var parentNode = (IOutputGameObject) parent;
                _inputValue1 = parentNode.GetOutputGameObject();
                _hasValue1 = true;
            }
            else
            {
                return State.SUCCESS;
            }

            if (_hasValue1 && _hasValue2)
            {
                if (_inputValue1 == null)
                {
                    return State.FAILURE;
                }

                switch (FieldName)
                {
                    case FieldNameEnum.ACTIVE:
                        if (_inputValue1.TryGetComponent<PoolableObject>(out var poolableObject))
                        {
                            if (_inputValue2 > 0.5f)
                            {
                                internalState.EventBus.CallEvent(new ActivateObjectEvent() { Target = poolableObject });
                            }
                            else
                            {
                                internalState.EventBus.CallEvent(new DeactivateObjectEvent() { Target = poolableObject });
                            }
                        }
                        else
                        {
                            _inputValue1.SetActive(_inputValue2 > 0.5f);
                        }
                    break;
                }

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