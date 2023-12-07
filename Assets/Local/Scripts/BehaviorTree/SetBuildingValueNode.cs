using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class SetBuildingValueNode : BehaviorCompositeNode
    {
        public enum FieldNameEnum {
            LEVEL = 0,
            ACTIVE = 1,
            UPGRADE_AREA_STATE = 2
        }

        public FieldNameEnum FieldName;

        private Building _inputValue1;
        private float _inputValue2;
        private bool _hasValue1;
        private bool _hasValue2;

        public SetBuildingValueNode()
        {
            Input1Type = typeof(Building);
            Input2Type = typeof(float);
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
                _inputValue2 = parentNode.GetOutputFloat();
                _hasValue2 = true;
            }
            else if (inputIndex == 0 && parent is IOutputBuilding)
            {
                var parentNode = (IOutputBuilding) parent;
                _inputValue1 = parentNode.GetOutputBuilding();
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
                    case FieldNameEnum.LEVEL:
                        _inputValue1.Level = (int) Mathf.Round(_inputValue2);
                    break;
                    case FieldNameEnum.ACTIVE:
                        _inputValue1.gameObject.SetActive(_inputValue2 > 0.5f);
                    break;
                    case FieldNameEnum.UPGRADE_AREA_STATE:
                        if (_inputValue1.UpgradeArea != null)
                        {
                            _inputValue1.UpgradeArea.gameObject.SetActive(_inputValue2 > 0.5f);
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