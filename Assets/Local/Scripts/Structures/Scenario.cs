using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Scenario: MonoBehaviour
    {
        [Header("Config")]
        public List<ScenarioStage> Stages;
        
        [Header("Runtime")]
        public int LastStageIndex;
    }
}
