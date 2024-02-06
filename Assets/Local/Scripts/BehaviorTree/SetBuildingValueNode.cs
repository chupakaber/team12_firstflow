using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class SetBuildingValueNode : BehaviorCompositeNode
    {
        [HideInInspector]
        public override string Section { get { return "Action"; } }

        public enum FieldNameEnum {
            LEVEL = 0,
            ACTIVE = 1,
            UPGRADE_AREA_STATE = 2,
            PRODUCTION_LIMIT = 3,
            ITEM_COST = 4,
            PRODUCE_ITEMS_PER_CYCLE = 5,
            RESOURCE_LIMIT = 6,
            BASE_PRODUCTION_LIMIT = 7,
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
                        internalState.EventBus.CallEvent(new LevelUpEvent() { Target = _inputValue1 });
                    break;
                    case FieldNameEnum.ACTIVE:
                        if (_inputValue2 > 0.5f)
                        {
                            internalState.EventBus.CallEvent(new ActivateObjectEvent() { Target = _inputValue1 });
                        }
                        else
                        {
                            internalState.EventBus.CallEvent(new DeactivateObjectEvent() { Target = _inputValue1 });
                        }
                    break;
                    case FieldNameEnum.UPGRADE_AREA_STATE:
                        if (_inputValue1.UpgradeArea != null)
                        {
                            _inputValue1.UpgradeArea.gameObject.SetActive(_inputValue2 > 0.5f);
                        }
                    break;
                    case FieldNameEnum.BASE_PRODUCTION_LIMIT:
                        _inputValue1.BaseProductionLimit = (int) Mathf.Round(_inputValue2);
                        internalState.EventBus.CallEvent(new BuildingConfigurationChangedEvent() { Building = _inputValue1 });
                    break;
                    case FieldNameEnum.PRODUCTION_LIMIT:
                        _inputValue1.ProductionLimit = (int) Mathf.Round(_inputValue2);
                        internalState.EventBus.CallEvent(new BuildingConfigurationChangedEvent() { Building = _inputValue1 });
                    break;
                    case FieldNameEnum.ITEM_COST:
                        _inputValue1.ItemCost = (int) Mathf.Round(_inputValue2);
                        internalState.EventBus.CallEvent(new BuildingConfigurationChangedEvent() { Building = _inputValue1 });
                    break;
                    case FieldNameEnum.PRODUCE_ITEMS_PER_CYCLE:
                        _inputValue1.ProductionItemAmountPerCycle = (int) Mathf.Round(_inputValue2);
                        internalState.EventBus.CallEvent(new BuildingConfigurationChangedEvent() { Building = _inputValue1 });
                    break;
                    case FieldNameEnum.RESOURCE_LIMIT:
                        _inputValue1.ResourceLimit = (int) Mathf.Round(_inputValue2);
                        internalState.EventBus.CallEvent(new BuildingConfigurationChangedEvent() { Building = _inputValue1 });
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