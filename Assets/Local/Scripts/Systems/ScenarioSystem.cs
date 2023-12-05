using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class ScenarioSystem: ISystem
    {
        public EventBus EventBus;
        public UIView UIView;
        public Scenario Scenario;

        public List<Character> Characters;
        public List<Building> Buildings;

        private const float CHECK_COOLDOWN = 1f;

        private float _lastCheckTime;

        public void EventCatch(ConstructionCompleteEvent newEvent)
        {
            foreach (var stage in Scenario.Stages)
            {
                if (stage.ReactOnEvent == GameEventType.CONSTRUCTION && !stage.Completed)
                {
                    var success = true;
                    foreach (var condition in stage.Conditions)
                    {
                        if (!CheckCondition(condition))
                        {
                            success = false;
                        }
                    }
                    
                    if (success)
                    {
                        stage.Completed = true;
                        foreach (var action in stage.Actions)
                        {
                            DoAction(action);
                        }
                    }
                }
            }
        }

        public void EventCatch(AddItemEvent newEvent)
        {
            foreach (var stage in Scenario.Stages)
            {
                if (stage.ReactOnEvent == GameEventType.ADD_ITEM && !stage.Completed)
                {
                    var success = true;
                    foreach (var condition in stage.Conditions)
                    {
                        if (!CheckCondition(condition))
                        {
                            success = false;
                        }
                    }
                    
                    if (success)
                    {
                        stage.Completed = true;
                        foreach (var action in stage.Actions)
                        {
                            DoAction(action);
                        }
                    }
                }
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            if (Time.time > _lastCheckTime + CHECK_COOLDOWN)
            {
                _lastCheckTime = Time.time;

                foreach (var stage in Scenario.Stages)
                {
                    if (stage.ReactOnEvent == GameEventType.FIXED_UPDATE && !stage.Completed)
                    {
                        var success = true;
                        foreach (var condition in stage.Conditions)
                        {
                            if (!CheckCondition(condition))
                            {
                                success = false;
                            }
                        }
                        
                        if (success)
                        {
                            stage.Completed = true;
                            foreach (var action in stage.Actions)
                            {
                                DoAction(action);
                            }
                        }
                    }
                }
            }
        }

        public bool CheckCondition(ScenarioStageCondition condition)
        {
            switch (condition.Type)
            {
                case ConditionType.BUILDING_LEVEL:
                    return Compare(condition.Building.Level, condition);
            }

            return true;
        }

        public void DoAction(ScenarioStageAction action)
        {
            switch (action.Type)
            {
                case ScenarioActionType.DEACTIVATE:
                    if (action.Transform != null)
                    {
                        action.Transform.gameObject.SetActive(false);
                    }
                    break;
                case ScenarioActionType.ACTIVATE:
                    if (action.Transform != null)
                    {
                        action.Transform.gameObject.SetActive(true);
                    }
                    break;
                case ScenarioActionType.ARROW_POINTER:
                    UIView.PointerArrowTargetPosition = action.Transform != null ? action.Transform.position : Vector3.zero;
                    UIView.PointerArrowTargetPositionOnNavMesh = Vector3.zero;
                    if (action.Transform.gameObject.TryGetComponent<Building>(out var building))
                    {
                        if (building.PickingUpArea != null)
                        {
                            UIView.PointerArrowTargetPositionOnNavMesh = building.PickingUpArea.transform.position;
                        }
                        else if (building.UpgradeArea != null)
                        {
                            UIView.PointerArrowTargetPositionOnNavMesh = building.UpgradeArea.transform.position;
                        }
                    }
                    break;
                case ScenarioActionType.SHOW_EMOJI:
                    // TODO: find character procedural
                    EventBus.CallEvent(new ShowEmojiEvent() { Character = Characters[0], SpriteIndex = (int) Mathf.Round(action.Value) });
                    break;
            }
        }

        private bool Compare(float value, ScenarioStageCondition condition)
        {
            switch (condition.CompareType)
            {
                case ConditionCompareType.GREATER:
                    return value - condition.Value > float.Epsilon;
                case ConditionCompareType.LESS:
                    return value - condition.Value < -float.Epsilon;
                case ConditionCompareType.EQUAL:
                    return Mathf.Abs(value - condition.Value) <= float.Epsilon;
            }
            return true;
        }
    }
}
