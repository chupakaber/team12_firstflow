using System;
using System.Collections.Generic;

namespace Scripts.BehaviorTree
{
    [Serializable]
    public class ScenarioState : IBehaviorState
    {
        // Global context
        public EventBus EventBus { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Character> Characters { get; set; }

        public SmartCharacter Player;

        // Entity state
        public float[] Timers = new float[32];
        public int[] States = new int[32];
    }
}