using System.Collections.Generic;
using Scripts.BehaviorTree;
using UnityEngine;

namespace Scripts
{
    public class Scenario: MonoBehaviour
    {
        [Header("Config")]
        public List<ScenarioStage> Stages;
        
        [Header("Runtime")]
        public int LastStageIndex;

        public BehaviorTreeRunner BehaviorTreeRunner;
    }
}
