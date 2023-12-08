using System;
using System.Collections.Generic;

namespace Scripts.BehaviorTree
{
    [Serializable]
    public class SmartCharacterState : IBehaviorState
    {
        // Global context
        public EventBus EventBus { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Character> Characters { get; set; }

        public SmartCharacter Self;

        // Entity state
        public float[] Timers = new float[32];
        public int[] States = new int[32];

        public float GetTimer(int index)
        {
            return Timers[index];
        }

        public int GetState(int index)
        {
            return States[index];
        }

        public void SetTimer(int index, float value)
        {
            Timers[index] = value;
        }

        public void SetState(int index, int value)
        {
            States[index] = value;
        }
    }
}