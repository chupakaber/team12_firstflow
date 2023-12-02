using System;
using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    [Serializable]
    public class ScenarioStage
    {
        public List<ScenarioStageCondition> Conditions;
        public List<ScenarioStageAction> Actions;
        public GameEventType ReactOnEvent;
        public bool Completed;
    }

    [Serializable]
    public class ScenarioStageCondition
    {
        public ConditionType Type;
        public ConditionCompareType CompareType;
        public Transform Transform;
        public Character Character;
        public Building Building;
        public float Value;
    }

    [Serializable]
    public class ScenarioStageAction
    {
        public ScenarioActionType Type;
        public Transform Transform;
        public Character Character;
        public Building Building;
        public float Value;
    }
}
